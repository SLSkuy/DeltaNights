/* ------------------------------------------------------------
 *  Author:  2023051604044 wanrui
 *  Date:  2025.12.23
 *  LastUpdate: 2025.12.23
 *
 *  客户端连接抽象
 *  每一个客户端对应一个ClientInfo
 *  记录客户端的各种信息
 * ------------------------------------------------------------ */

#pragma once

#include <QObject>
#include <QHostAddress>

class PlayerInfo;

class ClientInfo : public QObject
{
    Q_OBJECT
public:
    explicit ClientInfo(const QHostAddress& ip, quint16 port, quint32 clientID, QObject* parent = nullptr);

    // IP信息
    const QHostAddress& ip() const;
    quint16 port() const;
    quint32 clientID() const;

    // 网络心跳
    void updateLastActiveTime();
    quint64 lastActiveTime() const;

    // 绑定玩家
    void bindPlayer(PlayerInfo* player);
    PlayerInfo* getPlayer() const;
    void unbindPlayer();

private:
    QHostAddress m_ip;
    quint16 m_port = 0;
    quint32 m_clientID = 0;
    quint64 m_lastActive = 0;

    PlayerInfo* m_player = nullptr; // 客户端连接服务器后，登陆账号绑定玩家信息
};

