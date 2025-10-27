#pragma once

#include <QTcpServer>
#include <QTcpSocket>
#include <QHostAddress>
#include <QMap>

class DNServer : public QTcpServer
{
public:
    DNServer(QObject *prent = nullptr);
    ~DNServer();
    bool StartServer(const QHostAddress &address,quint16 port);

protected:
    void incomingConnection(qintptr socketDescriptor) override;

private slots:
    void OnClientDisconnected();
    void OnReadyRead();

private:
    QMap<qintptr, QTcpSocket*> m_clients;
};
