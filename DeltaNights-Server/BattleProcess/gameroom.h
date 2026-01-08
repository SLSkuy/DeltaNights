/* ------------------------------------------------------------
 *  Author:  2023051604044 wanrui
 *           2023051604046 wenrenqiang
 *  Date:  2025.12.23
 *  LastUpdate: 2026.1.8
 *
 *  游戏战局房间示例
 *  处理每一个战局的逻辑事件
 *  管理所有玩家实体，计算玩家的状态
 *  生成Protobuf同步事件，用于网络分发层发送
 * ------------------------------------------------------------ */

#pragma once

#include <QTimer>
#include <QObject>
#include <memory>
#include <unordered_map>

#include "../GameEvent/BattleSyncPackage.pb.h"
#include "../GameData/gamemap.h"
#include "playerentity.h"

class UdpEndpoint;
class ClientInfo;
class PlayerEntity;
class CollisionSystem;

enum class GameState
{
    Waiting,
    Running,
    Finished
};

struct GameRoomConfig
{
    int maxPlayers = 10;
    // TODO: 房间属性设置
};

class GameRoom : public QObject
{
    Q_OBJECT
public:
    GameRoom(quint32 roomID, QObject *parent = nullptr);
    ~GameRoom();

    // 玩家相关操作
    bool addPlayer(std::unique_ptr<PlayerEntity> player);
    bool removePlayer(quint32 clientID);
    void onPlayerInput(BattleSyncPackage::BattleSyncRequest* input);
    void playerTimeout(quint32 clientID);

    // 战局控制
    void start();
    void stop();

public:
    const std::unordered_map<quint32, std::unique_ptr<PlayerEntity>>& players() const { return m_players; }
    void roomName(QString roomname){m_roomName = roomname;}
    void roomType(QString roomtype){m_roomType = roomtype;}
    void roomIntroduction(QString roomintroduction){m_roomIntroduction = roomintroduction;}
    void roomOwnerName(QString ownername){m_ownerName = ownername;}
    void Max(int max){m_max= max;}
    void PlayerCount(int num){m_playerCount= num;}
    bool isRoomFull();
    //std::unordered_map<quint32, PlayerInfo*> teamWithFewPlayers();
    quint32 addInFewPlayersTeam(PlayerInfo *player);

    QString getRoomName(){return m_roomName;}
    QString getRoomType(){return m_roomType;}
    QString getRoomIntroduction(){return m_roomIntroduction;}
    QString getRoomOwnerName(){return m_ownerName;}
    //void addNum(){m_num++;}
    int getMax(){return m_max;}
    int getPlayerCount(){return m_playerCount;}

signals:
    void battleSync(quint32 roomID, BattleSyncPackage::BattleSyncResponse* response);  // 发送新Tick信号，由接收者处理每一Tick产生的Protobuf事件

private:
    void onTick();  // 战局逻辑更新，生成Protobuf事件
    void generateSyncPackage(); // 生成Protobuf同步包

private:
    // ========== 房间数据 ==========
    quint32 m_roomID;
    int m_playerCount = 0;
    GameState m_state = GameState::Waiting;
    GameRoomConfig m_config;

    QString m_roomName;
    QString m_roomType;
    QString m_roomIntroduction;
    QString m_ownerName;//房主
    int m_max=6;//房间最大人数默认为6
    quint32 m_teamAcount = 0;
    quint32 m_teamBcount = 0;
    //int m_num=0;//房间实时人数

    //QString _teamA[];

    // ========== Tick处理 ==========
    quint32 m_tick;
    float m_deltaTime;
    int m_tickRate = 64;
    int m_syncRate = 4; // 同步包发送频率，每多少tick发送一次包
    int m_lastSync = 0;
    QTimer* _timer = nullptr; // Tick计时器

    // ========== 世界模拟 ==========
    GameMap m_battleMap;    // 战局地图
    CollisionSystem* _collisionSystem = nullptr;  // 碰撞系统

    // ========== 玩家数据处理 ==========
    std::unordered_map<quint32, std::unique_ptr<PlayerEntity>> m_players; // clientID -> PlayerEntity
    std::unordered_map<quint32, PlayerInput> m_inputBuffer; // 每次同步如果有输入则覆盖PlayerEntity，没有则保持PlayerEntity中的输入
    std::unordered_map<quint32,PlayerInfo*> _teamA;//房间内分为队伍A和队伍B两个阵营
    std::unordered_map<quint32,PlayerInfo*> _teamB;
};
