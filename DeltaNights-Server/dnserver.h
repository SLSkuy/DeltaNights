#pragma once

#include <QTcpServer>
#include <QTcpSocket>
#include <QHostAddress>
#include <QThread>
#include <QMap>

class MsgSender;

class DNServer : public QTcpServer
{
    Q_OBJECT
public:
    DNServer(QObject *prent = nullptr);
    ~DNServer();
    bool StartServer(const QHostAddress &address,quint16 port);
    void SendMessage(QString content);

protected:
    void incomingConnection(qintptr socketDescriptor) override;

signals:
    void closeServer();

private slots:
    void OnClientDisconnected();
    void OnReadyRead();

private:
    QMap<qintptr, QTcpSocket*> m_clients;
    QThread *sendThread;
    MsgSender *msgsender;
};
