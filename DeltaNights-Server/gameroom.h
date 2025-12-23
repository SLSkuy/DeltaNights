/* ------------------------------------------------------------
 *  Author:  2023051604044 wanrui
 *  Date:  2025.12.23
 *  LastUpdate: 2025.12.23
 *
 *  游戏战局房间示例
 *  处理每一个战局的逻辑事件
 *  管理所有玩家实体，计算玩家的状态
 * ------------------------------------------------------------ */

#pragma once

#include <QObject>
#include <unordered_map>

class UdpEndpoint;
class ClientInfo;
class PlayerEntity;

class GameRoom : public QObject
{
    Q_OBJECT
public:
    GameRoom(QObject *parent = nullptr);
private:
    int m_roomCode;

    int m_playerCount = 0;

    std::unordered_map<quint32, ClientInfo*> m_clients;    // uuid -> ClientInfo
    std::unordered_map<quint32, PlayerEntity*> m_players; // uuid -> PlayerEntity

    UdpEndpoint* _udp;
};
