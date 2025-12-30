/* ------------------------------------------------------------
 *  Author:  2023051604044 wanrui
 *  Date:  2025.12.23
 *  LastUpdate: 2025.12.30
 *
 *  客户端管理
 *  维护所有客户端的连接
 * ------------------------------------------------------------ */

#include "ClientManager.h"
#include "ClientInfo.h"
#include "../Logger/logger.h"

#include <QDateTime>

ClientManager::ClientManager(QObject* parent)
    : QObject(parent)
{
}

QString ClientManager::makeKey(const QHostAddress& ip, quint16 port) const
{
    return ip.toString() + ":" + QString::number(port);
}

void ClientManager::createNewClient(QTcpSocket* socket)
{
    // 若存在客户端，则直接返回
    auto it = m_clientsByTcp.find(socket);
    if (it != m_clientsByTcp.end())
    {
        it->second->updateLastActiveTime();
        return;
    }

    // 创建新 ClientInfo
    ClientInfo* client = new ClientInfo(socket, m_nextClientID++, this);

    m_clientsByID.emplace(client->clientID(), client);
    m_clientsByTcp.emplace(socket, client);

    Logger::Info() << "[ClientManager]: New client connect on " << socket->peerAddress().toString();
}

void ClientManager::bindClientUdpPort(QTcpSocket* socket, quint16 port)
{
    ClientInfo* client = findClientByTcp(socket);

    if(!client)
    {
        Logger::Error() << "[ClientManager]: Failed to bind client udp port, can not find client";
        return;
    }

    client->bindUdpPort(port);
    m_clientsByUdp[makeKey(client->ip(), client->port())] = client;
}

ClientInfo* ClientManager::findClientByID(quint32 clientID)
{
    auto it = m_clientsByID.find(clientID);
    return it != m_clientsByID.end() ? it->second : nullptr;
}

ClientInfo* ClientManager::findClientByTcp(QTcpSocket* socket)
{
    auto it = m_clientsByTcp.find(socket);
    return it != m_clientsByTcp.end() ? it->second : nullptr;
}

ClientInfo* ClientManager::findClientByUdp(const QHostAddress& ip, quint16 port) const
{
    const QString key = makeKey(ip, port);

    auto it = m_clientsByUdp.find(key);
    return it != m_clientsByUdp.end() ? it->second : nullptr;
}

void ClientManager::removeClientById(quint32 clientId)
{
    auto it = m_clientsByID.find(clientId);
    if (it == m_clientsByID.end())
        return;

    ClientInfo* client = it->second;

    // 清理 TCP 索引
    if (client->tcpSocket())
        m_clientsByTcp.erase(client->tcpSocket());

    // 清理 UDP 索引
    if (client->port())
        m_clientsByUdp.erase(makeKey(client->ip(), client->port()));

    client->deleteLater();
    m_clientsByID.erase(it);
}


void ClientManager::removeTimeoutClients(quint64 timeout)
{
    const quint64 now = QDateTime::currentMSecsSinceEpoch();

    for (auto it = m_clientsByID.begin(); it != m_clientsByID.end();)
    {
        ClientInfo* client = it->second;
        if (now - client->lastActiveTime() > timeout)
        {
            m_clientsByTcp.erase(client->tcpSocket()); // 删除TCP索引
            m_clientsByUdp.erase(makeKey(client->ip(),client->port()));    // 删除UDP索引
            client->deleteLater();  // 删除客户端

            it = m_clientsByID.erase(it);   // 删除ID索引
        }
        else
        {
            ++it;
        }
    }
}
