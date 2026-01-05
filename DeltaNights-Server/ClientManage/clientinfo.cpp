/* ------------------------------------------------------------
 *  Author:  2023051604044 wanrui
 *  Date:  2025.12.23
 *  LastUpdate: 2025.12.31
 *
 *  客户端连接抽象
 *  每一个客户端对应一个ClientInfo
 *  记录客户端的各种信息
 * ------------------------------------------------------------ */

#include "clientinfo.h"
#include <QDateTime>

ClientInfo::ClientInfo(QTcpSocket* socket, quint32 clientID, QObject* parent)
    : QObject(parent)
    , _tcp(socket)
    , m_clientID(clientID)
{
    updateLastActiveTime();
}

void ClientInfo::bindUdpPort(quint16 port)
{
    m_port = port;
    m_ip = _tcp->peerAddress();
}

const QHostAddress& ClientInfo::ip()
{
    return m_ip;
}

quint16 ClientInfo::port() const
{
    return m_port;
}

quint32 ClientInfo::clientID() const
{
    return m_clientID;
}

QTcpSocket* ClientInfo::tcpSocket() const
{
    return _tcp;
}

void ClientInfo::updateLastActiveTime()
{
    m_lastActive = QDateTime::currentMSecsSinceEpoch();
}

const quint64& ClientInfo::lastActiveTime() const
{
    return m_lastActive;
}

void ClientInfo::bindPlayer(PlayerInfo* player)
{
    m_player = player;
}

PlayerInfo* ClientInfo::getPlayer() const
{
    return m_player;
}

void ClientInfo::unbindPlayer()
{
    m_player = nullptr;
}
