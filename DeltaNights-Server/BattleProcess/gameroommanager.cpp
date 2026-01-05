/* ------------------------------------------------------------
 *  Author:  2023051604044 wanrui
 *  Date:  2025.12.23
 *  LastUpdate: 2026.1.2
 *
 *  游戏战局房间管理
 *  处理房间的创建、销毁、玩家与房间之间的交互逻辑
 * ------------------------------------------------------------ */

#include "memory"

#include "../Logger/logger.h"
#include "../ClientManage/playerinfo.h"
#include "playerentity.h"
#include "gameroommanager.h"
#include "gameroom.h"
#include "../ClientManage/clientinfo.h"
#include "../ClientManage/clientmanager.h"

GameRoomManager::GameRoomManager(ClientManager* clientMgr, QObject *parent)
    : _clientManager(clientMgr), QObject(parent)
{

}

/* ------------------------------------------------------------
 * 房间管理操作
 * ------------------------------------------------------------ */
GameRoom* GameRoomManager::createGameRoom()
{
    while(m_rooms.count(m_nextRoomID))    // 查找空闲战局ID
    {
        ++m_nextRoomID;
    }

    GameRoom* newRoom = new GameRoom(m_nextRoomID, this);   // 由Qt父子系统管理生命周期
    m_rooms[m_nextRoomID] = newRoom;

    connect(newRoom, &GameRoom::battleSync, this, &GameRoomManager::battleSyncResponse);
    connect(_clientManager,&ClientManager::clientTimeout,newRoom,&GameRoom::playerTimeout);

    Logger::Info() << "[GameRoomManager]: " << "Create new game room with ID: " << m_nextRoomID++;
    return newRoom;
}

GameRoom* GameRoomManager::findGameRoomByID(quint32 roomID)
{
    if(m_rooms.count(roomID))
    {
        return m_rooms[roomID];
    }

    Logger::Error() << "[GameRoomManager]: " << "No game room with ID: " << roomID;
    return nullptr;
}

void GameRoomManager::disposeGameRoom(quint32 roomID)
{
    GameRoom* roomToDispose = findGameRoomByID(roomID);

    if(!roomToDispose)
    {
        Logger::Error() << "[GameRoomManager]: " << "No game room with ID: " << roomID;
        return;
    }

    disconnect(roomToDispose, &GameRoom::battleSync, this, &GameRoomManager::battleSyncResponse);
    disconnect(_clientManager,&ClientManager::clientTimeout,roomToDispose,&GameRoom::playerTimeout);

    m_rooms.erase(roomID);
    roomToDispose->deleteLater();
    Logger::Info() << "[GameRoomManager]: " << "Dispose game room with ID: " << roomID;
}

/* ------------------------------------------------------------
 * 玩家交互操作
 * ------------------------------------------------------------ */
bool GameRoomManager::joinGameRoom(quint32 roomID, PlayerInfo* player)
{
    GameRoom* gameRoom = findGameRoomByID(roomID);
    if (!gameRoom)
    {
        Logger::Error() << "[GameRoomManager]: " << "No game room with ID: " << roomID;
        return false;
    }

    auto entity = std::make_unique<PlayerEntity>(player);
    entity->bindClient(_clientManager->findClientByID(player->uuid())); // 绑定客户端

    return gameRoom->addPlayer(std::move(entity));
}

bool GameRoomManager::leaveGameRoom(quint32 roomID, PlayerInfo* player)
{
    GameRoom* gameRoom = findGameRoomByID(roomID);
    if (!gameRoom)
    {
        Logger::Error() << "[GameRoomManager]: " << "No game room with ID: " << roomID;
        return false;
    }

    return gameRoom->removePlayer(player->uuid());
}

void GameRoomManager::playerSyncRequest(BattleSyncPackage::BattleSyncRequest* input)
{
    if(input == nullptr) return;

    findGameRoomByID(input->roomid())->onPlayerInput(input);
}

void GameRoomManager::battleSyncResponse(quint32 roomID, BattleSyncPackage::BattleSyncResponse* pkg)
{
    GameRoom* room = findGameRoomByID(roomID);
    if(room)
    {
        for(auto& it:room->players())
        {
            ClientInfo* info = it.second->client();
            if(!info) continue;

            // 给每一个客户端发送战局同步包
            emit battleSyncGenerated(info->ip(), info->port(), pkg);
        }
    }
}
