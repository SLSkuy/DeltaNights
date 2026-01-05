/* ------------------------------------------------------------
 *  Author:  2023051604044 wanrui
 *  Date:  2025.12.23
 *  LastUpdate: 2025.12.31
 *
 *  TCP封装头文件
 *
 *  功能简述：
 *  - 封装 QTcpServer / QTcpSocket
 *  - 提供统一的连接、断开、消息收发接口
 *  - 用于登录、房间、可靠控制指令
 * ------------------------------------------------------------ */

#include <QHostAddress>
#include <QtEndian>

#include "tcpendpoint.h"
#include "../Logger/logger.h"

TcpEndpoint::TcpEndpoint(QObject* parent)
    : QObject(parent)
{
}

TcpEndpoint::~TcpEndpoint()
{
    for (auto* sock : m_clients)
    {
        sock->disconnectFromHost();
        sock->deleteLater();
    }

    if(_server)
    {
        _server->close();
    }
}

bool TcpEndpoint::listen(quint16 port, QHostAddress address)
{
    // 延迟创建TCP，使Socket归属于网络线程
    if(!_server)
    {
        _server = new QTcpServer(this);
        connect(_server, &QTcpServer::newConnection,this, &TcpEndpoint::onNewConnection);
    }

    if(!_sendTimer)
    {
        _sendTimer = new QTimer(this);
        connect(_sendTimer, &QTimer::timeout, this, &TcpEndpoint::processSendQueue);
        _sendTimer->start(1000 / m_tcpRate);
        Logger::Info() << "[TcpEndPoint]: TCP send Rate " << m_tcpRate << " Hz";
    }

    Logger::Info() << "[TcpEndpoint]: TCP Listen on " << address.toString() << ":" << port;
    return _server->listen(address, port);
}

void TcpEndpoint::onNewConnection()
{
    while (_server->hasPendingConnections())
    {
        QTcpSocket* socket = _server->nextPendingConnection();
        m_clients.insert(socket);
        m_receiveBuffers[socket] = QByteArray();

        connect(socket, &QTcpSocket::readyRead, this, &TcpEndpoint::onSocketReadyRead);
        connect(socket, &QTcpSocket::disconnected, this, &TcpEndpoint::onSocketDisconnected);

        emit clientConnected(socket);
    }
}

void TcpEndpoint::onSocketReadyRead()
{
    auto* socket = qobject_cast<QTcpSocket*>(sender());
    if (!socket) return;

    QByteArray& buffer = m_receiveBuffers[socket];
    buffer.append(socket->readAll());

    while (true)
    {
        if (buffer.size() < 4)
            return;

        // 去除头部字节并转换为大端编码
        qint32 bodyLen = qFromBigEndian<qint32>(reinterpret_cast<const char*>(buffer.constData()));

        if (bodyLen <= 0 || bodyLen > 10 * 1024 * 1024)
        {
            Logger::Error() << "[TcpEndpoint]: Invalid TCP bodyLen:" << bodyLen;
            buffer.clear();
            return;
        }

        if (buffer.size() < 4 + bodyLen)
            return;

        QByteArray data = buffer.mid(4, bodyLen);
        buffer.remove(0, 4 + bodyLen);

        emit messageReceived(socket, data);
    }
}


void TcpEndpoint::onSocketDisconnected()
{
    auto* socket = qobject_cast<QTcpSocket*>(sender());
    if (!socket) return;

    m_clients.erase(socket);
    m_receiveBuffers.erase(socket);

    emit clientDisconnected(socket);
    socket->deleteLater();
}

void TcpEndpoint::send(QTcpSocket* socket, const QByteArray& data)
{
    if (!socket || socket->state() != QAbstractSocket::ConnectedState)
        return;

    // 封装加入发送队列处理
    QMutexLocker lock(&m_sendMutex);
    m_sendQueue.enqueue({socket, std::move(data)});
}

void TcpEndpoint::processSendQueue()
{
    QQueue<TcpMessage> tcpMsgs;

    QMutexLocker lock(&m_sendMutex);
    tcpMsgs.swap(m_sendQueue);
    while (!tcpMsgs.isEmpty())
    {
        auto it = tcpMsgs.dequeue();

        // 确保客户端连接
        if (!it.socket || it.socket->state() != QAbstractSocket::ConnectedState)
            continue;

        it.socket->write(it.data);
    }
}
