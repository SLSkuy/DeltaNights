/* ------------------------------------------------------------
 *  Author:  2023051604044 wanrui
 *  Date:  2025.12.23
 *  LastUpdate: 2026.1.2
 *
 *  客户端管理
 *  维护所有客户端的连接
 * ------------------------------------------------------------ */

#pragma once

#include <QObject>
#include <QHostAddress>
#include <unordered_map>
#include <QTcpSocket>

#include "../GameEvent/SyncPackage.pb.h"

class ClientInfo;

class ClientManager : public QObject
{
    Q_OBJECT
public:
    explicit ClientManager(QObject* parent = nullptr);

    void createNewClient(QTcpSocket* socket);
    void clientBindUdpPort(QTcpSocket* socket, quint16 port);
    ClientInfo* findClientByID(quint32 clientID);
    ClientInfo* findClientByTcp(QTcpSocket* socket);
    ClientInfo* findClientByUdp(const QHostAddress& ip, quint16 port) const;

    // ===== 超时处理 =====
    void updateClientLastActive(QTcpSocket* socket);
    void removeTimeoutClients();

    // ===== 控制台命令 =====
    void printClientsInfo();

signals:
    void clientConnectResponse(QTcpSocket* socket, const SyncPackage::RemoteSyncPackage& pkg);

private:
    bool removeClientById(quint32 clientId);
    QString makeKey(const QHostAddress& ip, quint16 port) const;

private:
    int m_timeToRemove = 5000; // 心跳时间上限（ms），超过则删除客户端
    quint32 m_nextClientID = 0;

    std::unordered_map<quint32, ClientInfo*> m_clientsByID;    // 主索引：逻辑客户端ID
    std::unordered_map<QTcpSocket*, ClientInfo*> m_clientsByTcp;    // 辅助索引：TCP Socket
    std::unordered_map<QString, ClientInfo*> m_clientsByUdp;    // 辅助索引：UDP Endpoint（IP + Port）
};
