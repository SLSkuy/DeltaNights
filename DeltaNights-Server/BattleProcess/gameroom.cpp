/* ------------------------------------------------------------
 *  Author:  2023051604044 wanrui
 *  Date:  2025.12.23
 *  LastUpdate: 2025.12.30
 *
 *  游戏战局房间示例
 *  处理每一个战局的逻辑事件
 *  计算玩家的状态
 * ------------------------------------------------------------ */

#include "gameroom.h"
#include "playerentity.h"
#include "../CollisionSystem/collisionsystem.h"
#include "../Logger/logger.h"

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
 * 玩家管理操作
 * -------------------------------------------------- */
bool GameRoom::addPlayer(std::unique_ptr<PlayerEntity> player)
{
    auto id = player->uuid();
    if (m_players.count(id))
        return false;

    m_players.emplace(id, std::move(player));
    ++m_playerCount;
    return true;
}


bool GameRoom::removePlayer(quint32 uuid)
{
    if(!m_players.count(uuid))
    {
        Logger::Error() << "[GameRoom ID: " << m_roomID << "]: " << "No player with UUID: " << uuid;
        return false;
    }

    return m_players.erase(uuid);
}

/* --------------------------------------------------
 * 战局控制
 * -------------------------------------------------- */
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
    if (m_state != GameState::Running)
        return;

    // 将输入写入 PlayerEntity
    for (auto& [uuid, player] : m_players)
    {
        // 上一tick玩家有输入则更新PlayerEntity中的输入
        auto it = m_inputBuffer.find(uuid);
        if (it != m_inputBuffer.end())
        {
            player->input() = it->second;
        }else{
            // 上一tick没有输入，保持PlayerEntity中的输入缓存
        }
    }

    // TODO: 玩家输入预期逻辑（不含碰撞）
    for (auto& [_, player] : m_players)
        player->tick();

    // TODO: 碰撞与物理
    // _collisionSystem->update(m_battleMap, m_players);

    // TODO: 射线检测（如有开火）
    // _collisionSystem->raycast();

    // TODO: 生成同步包
    generateSyncPackage();

    ++m_tick;
}

void GameRoom::generateSyncPackage()
{
    // TODO: 生成Protobuf同步事件包
}
