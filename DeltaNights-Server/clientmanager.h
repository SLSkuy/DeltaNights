/* ------------------------------------------------------------
 *  Author:  2023051604044 wanrui
 *  Date:  2025.12.23
 *  LastUpdate: 2025.12.23
 *
 *  客户端管理
 *  维护所有客户端的连接
 * ------------------------------------------------------------ */

#pragma once

#include <QObject>
#include <QHostAddress>
#include <unordered_map>

class ClientInfo;

class ClientManager : public QObject
{
    Q_OBJECT
public:
    explicit ClientManager(QObject* parent = nullptr);
    ~ClientManager();

    ClientInfo* CreateNewClient(const QHostAddress& ip, quint16 port);
    ClientInfo* FindClient(const QHostAddress& ip, quint16 port) const;
    void RemoveClient(const QHostAddress& ip, quint16 port);

    void RemoveTimeoutClients(quint64 timeoutMs);

private:
    QString MakeKey(const QHostAddress& ip, quint16 port) const;

private:
    std::unordered_map<QString, ClientInfo*> m_clients;
    quint32 m_nextClientID = 1;
};
