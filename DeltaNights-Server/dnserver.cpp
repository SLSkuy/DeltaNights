#include "msgsender.h"
#include "dnserver.h"
#include "logger.h"

DNServer::DNServer(QObject *parent) : QTcpServer(parent)
{
    sendThread = new QThread();
    msgsender = new MsgSender();

    msgsender->moveToThread(sendThread);

    connect(sendThread, &QThread::started, msgsender, &MsgSender::SendMessage);
    connect(msgsender, &MsgSender::workFinished, sendThread, &QThread::quit);
    connect(msgsender, &MsgSender::workFinished, this, [=](){
        Logger::Log("Server is closing...");
        emit closeServer();
        for (auto &socket : m_clients)
        {
            socket->close();
        }
        this->close();
    });
    connect(msgsender, &MsgSender::send, this, [this](const QString &content){
        SendMessage(content);
    });

    sendThread->start();
}

DNServer::~DNServer()
{
    for(auto &socket:m_clients)
    {
        socket->deleteLater();
    }

    msgsender->deleteLater();
    sendThread->deleteLater();
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

void DNServer::SendMessage(QString content)
{
    for (auto it = m_clients.begin(); it != m_clients.end(); )
    {
        QTcpSocket *socket = it.value();
        if (socket && socket->state() == QAbstractSocket::ConnectedState)
        {
            socket->write(content.toUtf8());
            ++it;
        }
        else
        {
            it = m_clients.erase(it);  // 移除已断开的 socket
        }
    }
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
}
