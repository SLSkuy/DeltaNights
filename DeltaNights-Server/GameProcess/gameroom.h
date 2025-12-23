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

#include <QTimer>
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
    ~GameRoom();

    // 玩家相关操作
    void addPlayer(PlayerEntity* player);
    void removePlayer(quint32 uuid);

    // 战局控制
    void start(int tickRate = 64);
    void stop();

public:
    const std::unordered_map<quint32, PlayerEntity*>& players() const { return m_players; }

signals:
    void battleTick();  // 发送新Tick信号，由接收者处理每一Tick产生的Protobuf事件

private slots:
    void onTick();  // 战局逻辑更新，生成Protobuf事件

private:
    void generateSyncPackage(); // 生成Protobuf同步包

private:
    quint32 m_roomID;
    int m_playerCount = 0;

    quint32 m_tick;
    QTimer* _timer; // Tick计时器

    std::unordered_map<quint32, PlayerEntity*> m_players; // uuid -> PlayerEntity
};
