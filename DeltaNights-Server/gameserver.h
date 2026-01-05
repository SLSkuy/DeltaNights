/* ------------------------------------------------------------
 *  Author:  2023051604044 wanrui
 *  Date:  2025.12.23
 *  LastUpdate: 2025.12.30
 *
 *  功能：
 *  - 封装服务器所有核心模块
 *  - 负责模块初始化、连接、生命周期管理
 * ------------------------------------------------------------ */

#pragma once

#include <QObject>
#include <QThread>
#include <QTimer>

class TcpEndpoint;
class UdpEndpoint;
class NetworkDispatcher;

class ClientManager;
class PlayerInfoManager;

class GameRoomManager;

class GameServer : public QObject
{
    Q_OBJECT
public:
    explicit GameServer(QObject* parent = nullptr);
    ~GameServer();

    bool start(quint16 tcpPort, quint16 udpPort);
    void stop();

    void handleConsoleCommand(const QString& command);

private:
    void setupNetwork();
    void setupLogic();
    void setupConnections();

private:
    int m_logicRate = 60;   // 60Hz
    QTimer* _logicTimer = nullptr;  // 逻辑更新计时器

    // ================= 网络层 =================
    QThread* _netThread = nullptr;
    TcpEndpoint* _tcp = nullptr;
    UdpEndpoint* _udp = nullptr;
    NetworkDispatcher* _dispatcher = nullptr;

    // ================= 玩家层 =================
    ClientManager* _clientMgr = nullptr;
    PlayerInfoManager* _playerInfoMgr = nullptr;

    // ================= 战局层 =================
    GameRoomManager* _roomMgr = nullptr;
};
