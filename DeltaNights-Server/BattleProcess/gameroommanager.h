/* ------------------------------------------------------------
 *  Author:  2023051604044 wanrui
 *  Date:  2025.12.23
 *  LastUpdate: 2026.1.2
 *
 *  游戏战局房间管理
 *  处理房间的创建、销毁、玩家与房间之间的交互逻辑
 *  获取每个房间的Tick事件，并传给网络分发层进行网络同步
 * ------------------------------------------------------------ */

#pragma once

#include <QObject>
#include <qtcpsocket.h>
#include <unordered_map>
#include <QHostAddress>

#include "../GameEvent/BattleSyncPackage.pb.h"
#include "../GameEvent/SyncPackage.pb.h"

class GameRoom;
class PlayerInfo;
class ClientManager;

class GameRoomManager : public QObject
{
    Q_OBJECT
public:
    explicit GameRoomManager(ClientManager* clientMgr, QObject *parent = nullptr);

    // ========== 房间管理操作 ==========
    GameRoom* createGameRoom();
    GameRoom* findGameRoomByID(quint32 roomID);
    void disposeGameRoom(quint32 roomID);

    // ========== 玩家交互操作 ==========
    bool joinGameRoom(quint32 roomID, PlayerInfo* player);
    bool leaveGameRoom(quint32 roomID, PlayerInfo* player);
    void playerSyncRequest(BattleSyncPackage::BattleSyncRequest* input);
    void battleSyncResponse(quint32 roomID, BattleSyncPackage::BattleSyncResponse* pkg);
    void roomOwner(QTcpSocket*socket,QString roomname,QString roomtype,QString roomintroduction);
    void refreshGameRoom(QTcpSocket *socket);
    void assignRooms(QTcpSocket *socket,quint32 roomid);

signals:
    void battleSyncGenerated(const QHostAddress& addr, quint16 port, BattleSyncPackage::BattleSyncResponse* pkg);  // 转发各个战局的信号
    void roomCreateResponse(QTcpSocket* socket,const SyncPackage::RemoteSyncPackage& pkg);
    void refeshGameRoomResponse(QTcpSocket* socket,const SyncPackage::RemoteSyncPackage& pkg);
    void joinRoomResponse(QTcpSocket* socket,const SyncPackage::RemoteSyncPackage& pkg);

private:
    quint32 m_nextRoomID = 0;

    std::unordered_map<quint32, GameRoom*> m_rooms; // roomID -> GameRoom

    ClientManager* _clientManager;
};
