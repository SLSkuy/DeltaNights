/* ------------------------------------------------------------
 *  Author:  2023051604044 wanrui
 *  Date:  2025.12.23
 *  LastUpdate: 2025.12.28
 *
 *  TCP封装头文件
 *
 *  功能简述：
 *  - 封装 QTcpServer / QTcpSocket
 *  - 提供统一的连接、断开、消息收发接口
 *  - 用于登录、房间、可靠控制指令
 * ------------------------------------------------------------ */

#include <QHostAddress>

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
        // 无消息头，丢弃
        if (buffer.size() < 4)
            return;

        int bodyLen;
        memcpy(&bodyLen, buffer.constData(), 4);    // 取出4字节头部信息
        if (buffer.size() < 4 + bodyLen) return;

        QByteArray data = buffer.mid(4, bodyLen);
        buffer.remove(0, 4 + bodyLen);  //  去除已处理字节

        // 测试消息
        Logger::Info() << "Receive Message: " << QString::fromUtf8(data);
        send(socket,QString("服务器收到TCP连接").toUtf8());

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
    int len = data.size();

    packet.append(reinterpret_cast<const char*>(&len), sizeof(int));
    packet.append(data);

    socket->write(packet);
    return socket->flush();
}

