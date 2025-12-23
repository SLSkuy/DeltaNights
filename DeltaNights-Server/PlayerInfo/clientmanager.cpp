/* ------------------------------------------------------------
 *  Author:  2023051604044 wanrui
 *  Date:  2025.12.23
 *  LastUpdate: 2025.12.23
 *
 *  客户端管理
 *  维护所有客户端的连接
 * ------------------------------------------------------------ */

#include "ClientManager.h"
#include "ClientInfo.h"

#include <QDateTime>

ClientManager::ClientManager(QObject* parent)
    : QObject(parent)
{
}

ClientManager::~ClientManager()
{
    for(auto& it:m_clients)
    {
        it.second->deleteLater();
    }
}

QString ClientManager::makeKey(const QHostAddress& ip, quint16 port) const
{
    return ip.toString() + ":" + QString::number(port);
}

ClientInfo* ClientManager::createNewClient(const QHostAddress& ip, quint16 port)
{
    const QString key = makeKey(ip, port);

    // 若存在客户端，则直接返回客户端对象
    auto it = m_clients.find(key);
    if (it != m_clients.end())
    {
        it->second->updateLastActiveTime();
        return it->second;
    }

    // 创建新 ClientInfo
    ClientInfo* client = new ClientInfo(ip,port,m_nextClientID++,this);

    m_clients.emplace(key, client);
    return client;
}

ClientInfo* ClientManager::findClient(const QHostAddress& ip, quint16 port) const
{
    const QString key = makeKey(ip, port);

    auto it = m_clients.find(key);
    return it != m_clients.end() ? it->second : nullptr;
}

void ClientManager::removeClient(const QHostAddress& ip, quint16 port)
{
    const QString key = makeKey(ip, port);

    auto it = m_clients.find(key);
    if (it == m_clients.end())
        return;

    delete it->second;
    m_clients.erase(it);
}

void ClientManager::removeTimeoutClients(quint64 timeout)
{
    const quint64 now = QDateTime::currentMSecsSinceEpoch();

    for (auto it = m_clients.begin(); it != m_clients.end();)
    {
        ClientInfo* client = it->second;
        if (now - client->lastActiveTime() > timeout)
        {
            delete client;
            it = m_clients.erase(it);
        }
        else
        {
            ++it;
        }
    }
}
