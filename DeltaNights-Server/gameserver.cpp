/* ------------------------------------------------------------
*  Author:  2023051604044 wanrui
*           2023051604046 wenrenqiang
 *  Date:  2025.12.23
 *  LastUpdate: 2026.1.8
 *
 *  功能：
 *  - 封装服务器所有核心模块
 *  - 负责模块初始化、连接、生命周期管理
 * ------------------------------------------------------------ */

#include <QCoreApplication>

#include "gameserver.h"
#include "Logger/logger.h"

// Network
#include "NetworkProcess/tcpendpoint.h"
#include "NetworkProcess/udpendpoint.h"
#include "NetworkProcess/networkdispatcher.h"

// Client
#include "ClientManage/clientmanager.h"
#include "ClientManage/playerinfomanager.h"

// Battle
#include "BattleProcess/gameroommanager.h"

GameServer::GameServer(QObject* parent)
    : QObject(parent)
{
    setupNetwork();
    setupLogic();
    setupConnections();
}

GameServer::~GameServer()
{
    stop();
}

void GameServer::setupNetwork()
{
    _netThread = new QThread(this); // 生命周期交给Qt管理

    _tcp = new TcpEndpoint();
    _udp = new UdpEndpoint();

    _tcp->moveToThread(_netThread);
    _udp->moveToThread(_netThread);

    // 控制Socket的销毁时机
    connect(_netThread, &QThread::finished, _tcp, &QObject::deleteLater);
    connect(_netThread, &QThread::finished, _udp, &QObject::deleteLater);

    _dispatcher = new NetworkDispatcher(_udp,_tcp,this);

    _netThread->start();
}

void GameServer::setupLogic()
{
    // 通过Qt父子对象系统实现自动销毁
    _clientMgr = new ClientManager(this);
    _playerInfoMgr = new PlayerInfoManager(this);
    _roomMgr = new GameRoomManager(_clientMgr, this);
    _logicTimer = new QTimer(this);
}

// 测试使用
#include "ClientManage/playerinfo.h"
#include "BattleProcess/gameroom.h"
void GameServer::setupConnections()
{
    // TODO: 信号连接
    // 客户端连接处理
    //收
    connect(_dispatcher,&NetworkDispatcher::clientConnect,_clientMgr,&ClientManager::createNewClient);
    connect(_dispatcher,&NetworkDispatcher::clientBindUdpPort,_clientMgr,&ClientManager::clientBindUdpPort);
    connect(_dispatcher,&NetworkDispatcher::clientHeartBeat,_clientMgr,&ClientManager::updateClientLastActive);
    connect(_dispatcher,&NetworkDispatcher::clientLogin,_playerInfoMgr,&PlayerInfoManager::logIn);
    connect(_dispatcher,&NetworkDispatcher::clientCreateRoom,_roomMgr,&GameRoomManager::roomOwner);
    connect(_dispatcher,&NetworkDispatcher::clientRefresh, _roomMgr,&GameRoomManager::refreshGameRoom);
    connect(_dispatcher,&NetworkDispatcher::clientJoinRoom,_roomMgr,&GameRoomManager::assignRooms);
    //发
    connect(_clientMgr,&ClientManager::clientConnectResponse,_dispatcher,&NetworkDispatcher::sendTcpMessage);
    connect(_playerInfoMgr,&PlayerInfoManager::clientLoginResponse,_dispatcher,&NetworkDispatcher::sendTcpMessage);
    connect(_roomMgr,&GameRoomManager::roomCreateResponse,_dispatcher,&NetworkDispatcher::sendTcpMessage);
    connect(_roomMgr,&GameRoomManager::refeshGameRoomResponse,_dispatcher,&NetworkDispatcher::sendTcpMessage);
    connect(_roomMgr,&GameRoomManager::joinRoomResponse,_dispatcher,&NetworkDispatcher::sendTcpMessage);

    //
    connect(_playerInfoMgr,&PlayerInfoManager::clientBindPlayerInfo,_clientMgr,&ClientManager::clientBindPlayerInfo);



    // 测试使用
    /*connect(_clientMgr,&ClientManager::clientConnectResponse,this,[=](){
        GameRoom* room = _roomMgr->createGameRoom();
        PlayerInfo* player = new PlayerInfo(0);
        _roomMgr->joinGameRoom(0, player);
        room->start();
    });*/

    // 服务器逻辑更新
    connect(_logicTimer,&QTimer::timeout,_dispatcher,&NetworkDispatcher::processQueueMessage);
    connect(_logicTimer,&QTimer::timeout,_clientMgr,&ClientManager::removeTimeoutClients);

    // 战局逻辑连接
    connect(_dispatcher,&NetworkDispatcher::battleSyncRequest,_roomMgr,&GameRoomManager::playerSyncRequest);
    connect(_roomMgr,&GameRoomManager::battleSyncGenerated,_dispatcher,&NetworkDispatcher::sendUdpMessage);
}

bool GameServer::start(quint16 tcpPort, quint16 udpPort)
{
    // 跨线程启用TCP、UDP连接
    QMetaObject::invokeMethod(_tcp,[=]() { _tcp->listen(tcpPort); },Qt::QueuedConnection);
    QMetaObject::invokeMethod(_udp,[=]() { _udp->bind(udpPort); },Qt::QueuedConnection);

    // 启用逻辑更新计时器
    _logicTimer->start(1000 / m_logicRate);

    return true;
}

void GameServer::stop()
{
    // TODO: 关闭服务器
    if (_netThread)
    {
        _netThread->quit();
        _netThread->wait();
    }

    Logger::Info() << "[GameServer]: 服务器已关闭";
}

void GameServer::handleConsoleCommand(const QString& cmd)
{
    if (cmd == "stop")
    {
        Logger::Info() << "[GameServer]: Game server start to shut down";
        QCoreApplication::quit();
    }else if(cmd == "list clients")
    {
        Logger::Info() << "[GameServer]: Current connect clients: ";
        _clientMgr->printClientsInfo();
    }
    else
    {
        Logger::Warning() << "[GameServer]: Unknown command: " << cmd;
    }
}
