/* ------------------------------------------------------------
 *  Author:  2023051604044 wanrui
 *           2023051604046 wenrenqiang
 *  Date:  2025.12.23
 *  LastUpdate: 2026.1.8
 *
 *  游戏战局房间示例
 *  处理每一个战局的逻辑事件
 *  计算玩家的状态
 * ------------------------------------------------------------ */

#include "gameroom.h"
#include "playerentity.h"
#include "../CollisionSystem/collisionsystem.h"
#include "../Logger/logger.h"
#include "../ObjectPool/protopool.h"
#include "../ClientManage/clientinfo.h"

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
    auto id = player->client()->clientID();
    if (m_players.count(id))
        return false;

    m_players.emplace(id, std::move(player));
    ++m_playerCount;
    return true;
}


bool GameRoom::removePlayer(quint32 clientID)
{
    if(!m_players.count(clientID))
    {
        Logger::Error() << "[GameRoom ID: " << m_roomID << "]: " << "No player with clientID: " << clientID;
        return false;
    }

    return m_players.erase(clientID);
}

void GameRoom::onPlayerInput(BattleSyncPackage::BattleSyncRequest* input)
{
    if(input == nullptr) return;

    m_players[input->playerid()]->updateInput(input);
}

void GameRoom::playerTimeout(quint32 clientID)
{
    if(!m_players.count(clientID))
    {
        Logger::Error() << "[GameRoom ID: " << m_roomID << "]: " << "No player with clientID: " << clientID;
        return;
    }

    m_players[clientID]->unBind();
}

bool GameRoom::isRoomFull()
{
    return m_playerCount>=m_max;
}

quint32 GameRoom::addInFewPlayersTeam(PlayerInfo* player)
{
    //int i = 0;
    if(_teamB.size()>_teamA.size())
    {
        _teamB[m_teamBcount++] = player;
        return 2;
    }
    else
    {
        _teamA[m_teamAcount++] = player;
        return 1;
    }
}


/*std::unordered_map<quint32, PlayerInfo *> GameRoom::teamWithFewPlayers()
{
    return _teamB.size()>_teamA.size()?_teamB:_teamA;
}*/

/* --------------------------------------------------
 * 战局控制
 * -------------------------------------------------- */
void GameRoom::start()
{
    m_tick = 0;
    m_state = GameState::Running;
    _timer->start(1000 / m_tickRate);
}

void GameRoom::stop()
{
    _timer->stop();
    m_state = GameState::Finished;
}


void GameRoom::onTick()
{
    if (m_state != GameState::Running)
        return;

    if(m_lastSync == m_syncRate)
    {
        // 将输入写入 PlayerEntity
        for (auto& [uuid, player] : m_players)
        {
            // 同步时玩家有输入则更新PlayerEntity中的输入
            auto it = m_inputBuffer.find(uuid);
            if (it != m_inputBuffer.end())
            {
                player->input() = it->second;
            }else{
                // 同步时没有输入，保持PlayerEntity中的输入缓存
                // TODO: 清空输入
            }
        }
    }

    // TODO: 玩家输入预期逻辑（不含碰撞）
    for (auto& [_, player] : m_players)
        player->tick();

    // TODO: 碰撞与物理
    // _collisionSystem->update(m_battleMap, m_players);

    // TODO: 射线检测（如有开火）
    // _collisionSystem->raycast();

    // 生成同步包
    if(m_lastSync == m_syncRate)
    {
        m_lastSync = 0;
        generateSyncPackage();
    }else{
        ++m_lastSync;
    }

    ++m_tick;
}

void GameRoom::generateSyncPackage()
{
    // TODO: 生成Protobuf同步事件包
    using namespace BattleSyncPackage;

    BattleSyncResponse* response = ProtoPool::AcquireBattleResp();
    response->set_roomid(m_roomID);
    response->set_tick(m_tick);
    for (auto& it : m_inputBuffer)
    {
        const PlayerInput& input = it.second;

        PlayerState* state = response->add_states();

        state->set_playerid(it.first);

        // ===== 输入同步 =====
        auto* moveDir = state->mutable_movedir();
        moveDir->set_x(input.moveDir.x());
        moveDir->set_y(input.moveDir.y());

        state->set_jump(input.jump);
        state->set_yaw(input.yaw);
        state->set_pitch(input.pitch);
        state->set_activeskill(input.activeSkill);
        state->set_ultimateskill(input.ultimateSkill);

        // ===== 权威状态 =====
        auto* pos = state->mutable_position();
        auto* player = m_players[it.first].get();
        pos->set_x(player->position().x());
        pos->set_y(player->position().y());
        pos->set_z(player->position().z());

        auto* rot = state->mutable_eulaangle();
        rot->set_x(player->eulaAngle().x());
        rot->set_y(player->eulaAngle().y());
        rot->set_z(player->eulaAngle().z());
    }

    emit battleSync(m_roomID, response);
    ProtoPool::Release(response);
}
