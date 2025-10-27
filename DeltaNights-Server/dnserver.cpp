#include "dnserver.h"
#include "logger.h"

DNServer::DNServer(QObject *parent) : QTcpServer(parent)
{

}

DNServer::~DNServer()
{

}

bool DNServer::StartServer(const QHostAddress &address, quint16 port)
{
    Logger::Log("Server is staring...");

    if(!this->listen(address, port))
    {
        Logger::Error("Server start failed: " + this->errorString());
        Logger::Error("Server will exit later");
        return false;
    }

    Logger::Log("Server Listening on port: " + QString::number(port));
    return true;
}

void DNServer::incomingConnection(qintptr socketDescriptor)
{
    auto *socket = new QTcpSocket(this);
    if (!socket->setSocketDescriptor(socketDescriptor))
    {
        Logger::Error("Failed to set socket descriptor");
        socket->deleteLater();
        return;
    }

    m_clients[socketDescriptor] = socket;
    Logger::Log("New client connected: " + socket->peerAddress().toString()
                + ":" + QString::number(socket->peerPort()));

    // 接收数据时处理
    connect(socket, &QTcpSocket::readyRead, this, &DNServer::OnReadyRead);

    // 断开连接时处理
    connect(socket, &QTcpSocket::disconnected, this, &DNServer::OnClientDisconnected);
}

void DNServer::OnClientDisconnected()
{
    QTcpSocket* socket = qobject_cast<QTcpSocket*>(sender());
    if (!socket)
        return;

    qintptr descriptor = socket->socketDescriptor();
    Logger::Log("Client disconnected: " + socket->peerAddress().toString());

    m_clients.remove(descriptor);
    socket->deleteLater();
}

void DNServer::OnReadyRead()
{
    QTcpSocket *socket = qobject_cast<QTcpSocket *>(sender());
    if (!socket)
        return;

    QByteArray data = socket->readAll();
    QString msg = QString::fromUtf8(data);
    Logger::Log("Received from " + socket->peerAddress().toString() + ": " + msg);

    // 返回收到的消息给客户端
    socket->write("Server receive message: " + data);
}
