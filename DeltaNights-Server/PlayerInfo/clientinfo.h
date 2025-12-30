/* ------------------------------------------------------------
 *  Author:  2023051604044 wanrui
 *  Date:  2025.12.23
 *  LastUpdate: 2025.12.30
 *
 *  客户端连接抽象
 *  每一个客户端对应一个ClientInfo
 *  记录客户端的各种信息
 * ------------------------------------------------------------ */

#pragma once

#include <QObject>
#include <QHostAddress>
#include <QTcpSocket>

class PlayerInfo;

class ClientInfo : public QObject
{
    Q_OBJECT
public:
    explicit ClientInfo(QTcpSocket* socket, quint32 clientID, QObject* parent = nullptr);

public:
    // IP信息
    void bindUdpPort(quint16 port);
    quint16 port() const;
    QTcpSocket* tcpSocket() const;
    quint32 clientID() const;

    // 网络心跳
    void updateLastActiveTime();
    const quint64& lastActiveTime() const;

    // 绑定玩家
    PlayerInfo* getPlayer() const;
    void bindPlayer(PlayerInfo* player);
    void unbindPlayer();

private:
    quint32 m_clientID = 0;
    quint64 m_lastActive = 0;

    quint16 m_port = 0; // UDP端口
    QTcpSocket* m_tcp = nullptr;    // 连接时绑定TCP

    PlayerInfo* m_player = nullptr; // 客户端连接服务器后，登陆账号绑定玩家信息
};

