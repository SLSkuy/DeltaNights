/* ------------------------------------------------------------
 *  Author:  2023051604044 wanrui
 *  Date:  2025.12.23
 *  LastUpdate: 2025.12.23
 *
 *  游戏战局房间示例
 *  处理每一个战局的逻辑事件
 *  管理所有玩家实体，计算玩家的状态
 *  生成Protobuf同步事件，用于网络分发层发送
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
    GameRoom(quint32 roomID, QObject *parent = nullptr);

    // 玩家相关操作
    void addPlayer(PlayerEntity* player);
    void removePlayer(quint32 uuid);
    const std::unordered_map<quint32, PlayerEntity*>& getPlayers() const { return m_players; }

    // 战局逻辑更新
    void tick();    // 生成Protobuf事件

private:
    quint32 m_roomID;
    int m_playerCount = 0;

    std::unordered_map<quint32, PlayerEntity*> m_players; // uuid -> PlayerEntity
};
