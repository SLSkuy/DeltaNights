/* ------------------------------------------------------------
 *  Author:  2023051604044 wanrui
 *           2023051604046 wenrenqiang
 *  Date:  2025.12.23
 *  LastUpdate: 2026.1.7
 *
 *  网络分发器
 *  处理UDP、TCP的数据收发
 *  负责Protobuf事件的序列化与反序列化
 * ------------------------------------------------------------ */

#pragma once

#include <QObject>
#include <QMutex>
#include <QTcpSocket>
#include <QQueue>

#include "../GameEvent/ClientSyncPackage.pb.h"
#include "../GameEvent/SyncPackage.pb.h"
#include "../GameEvent/LobbySyncPackage.pb.h"
#include "../GameEvent/BattleSyncPackage.pb.h"

class UdpEndpoint;
class TcpEndpoint;
class GameRoom;

struct NetMessage
{
    // TCP消息使用
    QTcpSocket* socket;

    // UDP消息使用
    QHostAddress addr;
    quint16 port;

    // 字节流
    QByteArray data;
};

class NetworkDispatcher : public QObject
{
    Q_OBJECT
public:
    explicit NetworkDispatcher(UdpEndpoint* udp, TcpEndpoint* tcp, QObject* parent = nullptr);

    // 将网络线程获取数据加入队列处理
    void onTcpMessage(QTcpSocket* socket, const QByteArray& data);
    void onUdpMessage(const QHostAddress& addr, quint16 port, const QByteArray& data);
    void processQueueMessage();  // 处理队列中的字节流

    // 将发送数据传入网络线程
    void sendTcpMessage(QTcpSocket* socket,const SyncPackage::RemoteSyncPackage& pkg);
    void sendUdpMessage(const QHostAddress& addr, quint16 port, BattleSyncPackage::BattleSyncResponse* pkg);

signals:
    // 发送信号
    void sendTcp(QTcpSocket* socket, const QByteArray& data);
    void sendUdp(const QHostAddress& addr, quint16 port, const QByteArray& data);

    // 客户端相关事件
    void clientConnect(QTcpSocket* socket);
    void clientBindUdpPort(QTcpSocket* socket, quint16 port);
    void clientHeartBeat(QTcpSocket* socket);
    void clientLogin(QTcpSocket* socket,QString account,QString password);
    //void clientCreateRoom(LobbySyncPackage::RoomCreateRequest* pkg);
    void clientCreateRoom(QTcpSocket* socket,QString roomname,QString roomtype,QString roomintroduction);
    void clientRefresh(QTcpSocket* socket);

    // 战局同步事件
    void battleSyncRequest(BattleSyncPackage::BattleSyncRequest* pkg);
    void battleSyncResponse(BattleSyncPackage::BattleSyncResponse* pkg);

    // 发送各种事件信号
    void loginRequest();

private:
    void handleTcpPackage(QTcpSocket* socket, const QByteArray& data);
    void handleTcpClientPackage(QTcpSocket* socket, const ClientSyncPackage::ClientSyncRequest& pkg);
    void handleTcpLobbyPackage(QTcpSocket* socket, const LobbySyncPackage::LobbySyncRequest& pkg);
    void handleUdpPackage(const QHostAddress& addr, quint16 port, const QByteArray& data);

private:
    UdpEndpoint* _udp = nullptr;
    TcpEndpoint* _tcp = nullptr;

    // TODO: 消息队列
    // TODO: 网络线程将消息加入队列，等待主线程取出处理
    QMutex m_mutex;
    QQueue<NetMessage> m_tcpQueue;
    QQueue<NetMessage> m_udpQueue;
};
