/* ------------------------------------------------------------
 *  Author:  2023051604044 wanrui
 *  Date:  2025.12.23
 *  LastUpdate: 2026.1.2
 *
 *  客户端管理
 *  维护所有客户端的连接
 * ------------------------------------------------------------ */

#include "clientmanager.h"
#include "clientinfo.h"
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

    Logger::Info() << "[ClientManager]: New client connect from "
                   << socket->peerAddress().toString()
                   << ":" << socket->peerPort();

    // 创建回复Protobuf包
    using namespace SyncPackage;
    RemoteSyncPackage response;
    response.set_eventid(RemoteSyncEvent::ClientResponse);
    auto* type = response.mutable_clientpackage();
    type->set_eventid(ClientSyncPackage::RemoteClientEvent::ConnectResponse);
    auto* connectResponsePkg = type->mutable_connectresponse();
    connectResponsePkg->set_content(QString("服务器连接成功").toStdString());

    // 触发连接回复信号
    emit clientConnectResponse(socket, response);
}

void ClientManager::clientBindUdpPort(QTcpSocket* socket, quint16 port)
{
    ClientInfo* client = findClientByTcp(socket);

    if(!client)
    {
        Logger::Error() << "[ClientManager]: Failed to bind client udp port, can not find client";
        return;
    }

    client->bindUdpPort(port);
    m_clientsByUdp[makeKey(client->ip(), client->port())] = client;

    Logger::Info() << "[ClientManager]: Client "
                   << socket->peerAddress().toString()
                   << ":" << socket->peerPort()
                   << " bind udp port on " << port;
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

bool ClientManager::removeClientById(quint32 clientId)
{
    auto it = m_clientsByID.find(clientId);
    if (it == m_clientsByID.end())
    {
        Logger::Error() << "[ClientManager]: Fail to remove client with ID " << clientId;
        return false;
    }

    ClientInfo* client = it->second;

    // 清理 TCP 索引
    if (client->tcpSocket())
        m_clientsByTcp.erase(client->tcpSocket());

    // 清理 UDP 索引
    if (client->port())
        m_clientsByUdp.erase(makeKey(client->ip(), client->port()));

    client->deleteLater();
    m_clientsByID.erase(it);

    return true;
}

/* ============================================================
 * 超时处理
 * ============================================================ */
void ClientManager::updateClientLastActive(QTcpSocket* socket)
{
    ClientInfo* client = findClientByTcp(socket);
    if(client)
    {
        client->updateLastActiveTime();
    }
}

void ClientManager::removeTimeoutClients()
{
    const quint64 now = QDateTime::currentMSecsSinceEpoch();

    for (auto it = m_clientsByID.begin(); it != m_clientsByID.end();)
    {
        ClientInfo* client = it->second;
        if (now - client->lastActiveTime() > m_timeToRemove)
        {
            Logger::Info() << "[ClientManager]: Client "
                           << makeKey(client->ip(), client->port()) << " timeout";

            // 触发超时信号，让战局内实体与客户端断开联系
            emit clientTimeout(client->clientID());

            m_clientsByTcp.erase(client->tcpSocket()); // 删除TCP索引
            m_clientsByUdp.erase(makeKey(client->ip(), client->port()));    // 删除UDP索引
            it = m_clientsByID.erase(it);   // 删除ID索引
            client->deleteLater();  // 删除客户端
        }
        else
        {
            ++it;
        }
    }
}

/* ============================================================
 * 控制台命令
 * ============================================================ */
void ClientManager::printClientsInfo()
{
    for(const auto& it : m_clientsByID)
    {
        qDebug().noquote() << "[ClientInfo: ID:[" << it.first << "] IP:[" << it.second->ip().toString() << "]]";
    }
}
