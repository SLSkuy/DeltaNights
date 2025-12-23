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
    : QObject(parent), m_roomID(roomID)
{
}

void GameRoom::addPlayer(PlayerEntity* player)
{
    m_players[player->getUUID()] = player;
}

void GameRoom::removePlayer(quint32 uuid)
{
    if(m_players.count(uuid))
    {
        m_players.erase(uuid);
    }
}
