// server.cpp
#include "server.h"

Server::Server(QObject *parent) : QObject(parent)
{
    tcpServer = new QTcpServer(this);
    connect(tcpServer, &QTcpServer::newConnection, this, &Server::newConnection);
}

void Server::startServer(quint16 port)
{
    if(tcpServer->listen(QHostAddress::Any, port))
    {
        qDebug() << "Server started on port" << port;
    }
    else
    {
        qDebug() << "Server failed to start:" << tcpServer->errorString();
    }
}

void Server::newConnection()
{
    clientSocket = tcpServer->nextPendingConnection();
    connect(clientSocket, &QTcpSocket::readyRead, this, &Server::readyRead);
    connect(clientSocket, &QTcpSocket::disconnected, this, &Server::disconnected);

    qDebug() << "Client connected from" << clientSocket->peerAddress().toString();

    // 发送欢迎消息
    QString welcomeMsg = "Hello from Qt Server!";
    clientSocket->write(welcomeMsg.toUtf8());
}

void Server::readyRead()
{
    QByteArray data = clientSocket->readAll();
    QString message = QString::fromUtf8(data);
    qDebug() << "Received from client:" << message;

    // 回复客户端
    QString response = "Server received: " + message;
    clientSocket->write(response.toUtf8());
}

void Server::disconnected()
{
    qDebug() << "Client disconnected";
    clientSocket->deleteLater();
}
