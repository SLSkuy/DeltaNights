/* ------------------------------------------------------------
 *  Author:  2023051604044 wanrui
 *  Date:  2025.12.23
 *  LastUpdate: 2025.12.30
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

    Logger::Info() << "TCP Listen on " << address.toString() << ":" << port;
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

        qint32 bodyLen = qFromBigEndian<qint32>(
            reinterpret_cast<const uchar*>(buffer.constData())
            );

        if (bodyLen <= 0 || bodyLen > 10 * 1024 * 1024)
        {
            Logger::Error() << "Invalid TCP bodyLen:" << bodyLen;
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

bool TcpEndpoint::send(QTcpSocket* socket, const QByteArray& data)
{
    if (!socket || socket->state() != QAbstractSocket::ConnectedState)
        return false;

    QByteArray packet;
    qint32 len = data.size();
    qint32 beLen = qToBigEndian(len);   // 统一使用大端序列

    packet.append(reinterpret_cast<const char*>(&beLen), sizeof(qint32));
    packet.append(data);

    socket->write(packet);
    return socket->flush();
}
