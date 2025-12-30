/* ------------------------------------------------------------
 *  Author:  2023051604044 wanrui
 *  Date:  2025.12.23
 *  LastUpdate: 2025.12.30
 *
 *  客户端管理
 *  维护所有客户端的连接
 * ------------------------------------------------------------ */

#pragma once

#include <QObject>
#include <QHostAddress>
#include <unordered_map>
#include <QTcpSocket>

class ClientInfo;

class ClientManager : public QObject
{
    Q_OBJECT
public:
    explicit ClientManager(QObject* parent = nullptr);

    void createNewClient(QTcpSocket* socket);
    void bindClientUdpPort(QTcpSocket* socket, quint16 port);
    ClientInfo* findClientByID(quint32 clientID);
    ClientInfo* findClientByTcp(QTcpSocket* socket);
    ClientInfo* findClientByUdp(const QHostAddress& ip, quint16 port) const;
    void removeClientById(quint32 clientId);

    // 删除超时客户端
    void removeTimeoutClients(quint64 timeout);

private:
    QString makeKey(const QHostAddress& ip, quint16 port) const;

private:
    int m_timeToRemove = 5; // 心跳时间上限，超过则删除客户端
    quint32 m_nextClientID = 1;

    std::unordered_map<quint32, ClientInfo*> m_clientsByID;    // 主索引：逻辑客户端ID
    std::unordered_map<QTcpSocket*, ClientInfo*> m_clientsByTcp;    // 辅助索引：TCP Socket
    std::unordered_map<QString, ClientInfo*> m_clientsByUdp;    // 辅助索引：UDP Endpoint（IP + Port）
};
