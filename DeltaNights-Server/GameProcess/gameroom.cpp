/* ------------------------------------------------------------
 *  Author:  2023051604044 wanrui
 *  Date:  2025.12.23
 *  LastUpdate: 2025.12.23
 *
 *  游戏战局房间示例
 *  处理每一个战局的逻辑事件
 *  计算玩家的状态
 * ------------------------------------------------------------ */

#include "gameroom.h"
#include "playerentity.h"

GameRoom::GameRoom(quint32 roomID, QObject* parent)
    : QObject(parent)
    , m_roomID(roomID)
{
    _timer = new QTimer();

    connect(_timer, &QTimer::timeout, this, &GameRoom::onTick);
}


GameRoom::~GameRoom()
{
    _timer->deleteLater();
}

/* --------------------------------------------------
    玩家管理操作
-------------------------------------------------- */
void GameRoom::addPlayer(PlayerEntity* player)
{
    m_players[player->uuid()] = player;
}

void GameRoom::removePlayer(quint32 uuid)
{
    if(m_players.count(uuid))
    {
        m_players.erase(uuid);
    }
}

/* --------------------------------------------------
    战局控制
-------------------------------------------------- */
void GameRoom::start(int tickRate)
{
    m_tick = 0;
    _timer->start(1000 / tickRate);
}

void GameRoom::stop()
{
    _timer->stop();
}

void GameRoom::onTick()
{
    ++m_tick;

    // TODO: 更新逻辑
    for (auto& [id, player] : m_players)
    {
        // player->tick();
    }

    // TODO: 生成同步包
    generateSyncPackage();

    // TODO: 抛出信号，交给外部处理
    emit battleTick();
}

void GameRoom::generateSyncPackage()
{
    // TODO: 生成Protobuf同步事件包
}
