/* ------------------------------------------------------------
*  Author:  2023051604044 wanrui
 *  Date:  2025.12.23
 *  LastUpdate: 2025.12.30
 *
 *  功能：
 *  - 封装服务器所有核心模块
 *  - 负责模块初始化、连接、生命周期管理
 * ------------------------------------------------------------ */

#include "gameserver.h"
#include "Logger/logger.h"

// Network
#include "NetworkProcess/tcpendpoint.h"
#include "NetworkProcess/udpendpoint.h"
#include "NetworkProcess/networkdispatcher.h"

// Player
#include "PlayerInfo/clientmanager.h"
#include "PlayerInfo/playerinfomanager.h"

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
    _roomMgr = new GameRoomManager(this);
}

void GameServer::setupConnections()
{
    // TODO: 信号连接

    // 客户端连接处理
    connect(_dispatcher,&NetworkDispatcher::clientConnect,_clientMgr,&ClientManager::createNewClient);
}

bool GameServer::start(quint16 tcpPort, quint16 udpPort)
{
    // 跨线程启用TCP、UDP连接
    QMetaObject::invokeMethod(_tcp,[=]() { _tcp->listen(tcpPort); },Qt::QueuedConnection);
    QMetaObject::invokeMethod(_udp,[=]() { _udp->bind(udpPort); },Qt::QueuedConnection);

    connect(_tcp, &TcpEndpoint::messageReceived,
            _dispatcher, &NetworkDispatcher::onTcpMessage, Qt::QueuedConnection);

    connect(_udp, &UdpEndpoint::messageReceived,
            _dispatcher, &NetworkDispatcher::onUdpMessage, Qt::QueuedConnection);

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

    Logger::Info() << "服务器已关闭";
}
