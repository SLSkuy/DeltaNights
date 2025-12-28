/* ------------------------------------------------------------
 *  Author:  2023051604044 wanrui
 *  Date:  2025.12.23
 *  LastUpdate: 2025.12.28
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

    ClientInfo* createNewClient(const QHostAddress& ip, quint16 port);
    ClientInfo* findClient(const QHostAddress& ip, quint16 port) const;
    void removeClient(const QHostAddress& ip, quint16 port);

    // 删除超时客户端
    void removeTimeoutClients(quint64 timeout);

private:
    QString makeKey(const QHostAddress& ip, quint16 port) const;

private:
    quint32 m_nextClientID = 1;

    std::unordered_map<QString, ClientInfo*> m_clients;
};
