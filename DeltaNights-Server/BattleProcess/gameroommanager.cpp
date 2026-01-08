/* ------------------------------------------------------------
 *  Author:  2023051604044 wanrui
 *           2023051604046 wenrenqiang
 *  Date:  2025.12.23
 *  LastUpdate: 2026.1.7
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

void GameRoomManager::roomOwner(QTcpSocket*socket,QString roomname,QString roomtype,QString roomintroduction)
{
    GameRoom* room = createGameRoom();
    room->roomName(roomname);
    room->roomType(roomtype);
    room->roomIntroduction(roomintroduction);
    //room->addNum();//实时增加房间人数
    auto ownername= _clientManager->findClientByTcp(socket)->getPlayer()->nickname();
    room->roomOwnerName(ownername);
    //测试代码
    qDebug()<<"[GameRoomManager]"<<m_rooms[m_nextRoomID-1]->getRoomIntroduction();

    ClientInfo* client = _clientManager->findClientByTcp(socket);
    PlayerInfo* player = client->getPlayer();
    //PlayerInfo* player = new PlayerInfo();
    joinGameRoom(m_nextRoomID-1, player);


    using namespace SyncPackage;
    RemoteSyncPackage response;
    response.set_eventid(RemoteSyncEvent::LobbyResponse);
    auto* type = response.mutable_lobbypackage();
    type->set_eventid(LobbySyncPackage::RemoteLobbyEvent::Remote_Lobby_RoomCreate);
    auto *createRoomResponse=type->mutable_roomcreateresponse();
    createRoomResponse->set_roomid(m_nextRoomID-1);
    createRoomResponse->set_max(m_rooms[m_nextRoomID-1]->getMax());
    createRoomResponse->set_num(m_rooms[m_nextRoomID-1]->getPlayerCount());

    Logger::Info() <<"[GameRoomManager]"<<"roomid"<<"-"<<m_nextRoomID-1;
    emit roomCreateResponse(socket,response);

    room->start();
}

void GameRoomManager::refreshGameRoom(QTcpSocket *socket)
{
    quint32 i = 0;

    using namespace SyncPackage;
    RemoteSyncPackage response;
    response.set_eventid(RemoteSyncEvent::LobbyResponse);
    auto* type = response.mutable_lobbypackage();
    type->set_eventid(LobbySyncPackage::RemoteLobbyEvent::Remote_Lobby_Refresh);
    auto *refreshlistRoomResponse=type->mutable_refreshlistresponse();


    for(i=0; i< m_nextRoomID;i++)
    {
        auto *room = refreshlistRoomResponse->add_rooms();
        room->set_roomid(i);
        room->set_roomname(m_rooms[i]->getRoomName().toStdString());
        room->set_roomtype(m_rooms[i]->getRoomType().toStdString());
        room->set_owner(m_rooms[i]->getRoomOwnerName().toStdString());
        room->set_max(m_rooms[i]->getMax());
        room->set_num(m_rooms[i]->getPlayerCount());
    }


    Logger::Info() <<"[GameRoomManager]"<<"refresh";
    emit refeshGameRoomResponse(socket, response);
}

void GameRoomManager::assignRooms(QTcpSocket *socket, quint32 roomid)
{
    PlayerInfo *player = _clientManager->findClientByTcp(socket)->getPlayer();
    joinGameRoom(roomid,player);
    GameRoom *room = findGameRoomByID(roomid);

    if(room->isRoomFull()) {
        qDebug()<<"加入失败，房间已满";
        return;
    }

    quint32 team = room->addInFewPlayersTeam(player);

    using namespace SyncPackage;
    RemoteSyncPackage response;
    response.set_eventid(RemoteSyncEvent::LobbyResponse);
    auto* type = response.mutable_lobbypackage();
    type->set_eventid(LobbySyncPackage::RemoteLobbyEvent::Remote_Lobby_RoomJoin);
    auto *roomJoinResponse = type->mutable_roomjoinresponse();
    roomJoinResponse->set_roomid(roomid);
    roomJoinResponse->set_roomteam(team);


    emit joinRoomResponse(socket,response);
}
