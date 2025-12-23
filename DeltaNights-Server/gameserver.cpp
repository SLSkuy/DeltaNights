#include "gameserver.h"

// Network
#include "NetworkProcess/tcpendpoint.h"
#include "NetworkProcess/udpendpoint.h"
#include "NetworkProcess/networkdispatcher.h"

// Player
#include "PlayerInfo/clientmanager.h"
#include "PlayerInfo/playerinfomanager.h"

// Battle
#include "GameProcess/gameroommanager.h"

GameServer::GameServer(QObject* parent)
    : QObject(parent)
{
    setupNetwork();
    setupLogic();
    setupConnections();
}

GameServer::~GameServer()
{
    _dispatcher->deleteLater();
    _tcp->deleteLater();
    _udp->deleteLater();

    _clientMgr->deleteLater();
    _roomMgr->deleteLater();

    stop();
}

void GameServer::setupNetwork()
{
    _tcp = new TcpEndpoint(this);
    _udp = new UdpEndpoint(this);
    _dispatcher = new NetworkDispatcher(_udp,_tcp,this);
}

void GameServer::setupLogic()
{
    _clientMgr = new ClientManager(this);
    _playerInfoMgr = new PlayerInfoManager(this);

    _roomMgr = new GameRoomManager(this);
}

void GameServer::setupConnections()
{
    // TODO: 信号连接
}

bool GameServer::start(quint16 tcpPort, quint16 udpPort)
{
    if (!_tcp->listen(tcpPort))
        return false;

    if (!_udp->bind(udpPort))
        return false;

    return true;
}

void GameServer::stop()
{
    // TODO: 关闭服务器
}
