/* ------------------------------------------------------------
 *  Author:  2023051604044 wanrui
 *  Date:  2025.12.23
 *  LastUpdate: 2025.12.30
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

#include "../GameEvent/AckPackage.pb.h"

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

    void broadcastRoomFrame(GameRoom* room);    // 广播战局事件

signals:
    // 客户端相关事件
    void clientConnect(QTcpSocket* socket);
    void clientBindUdpPort(QTcpSocket* socket, quint16 port);
    void clientHeartBeat(QTcpSocket* socket);

    // 发送各种事件信号
    void loginRequest();

private:
    void handleTcpPackage(QTcpSocket* socket, const QByteArray& data);
    void handleTcpAckPackage(QTcpSocket* socket, const AckPackage::AckSyncRequest& pkg);
    void handleUdpPackage(const QHostAddress& addr, quint16 port, const QByteArray& data);

private:
    UdpEndpoint* _udp = nullptr;
    TcpEndpoint* _tcp = nullptr;

    QMutex _mutex;
    // TODO: 消息队列
    // TODO: 网络线程将消息加入队列，等待主线程取出处理
    QQueue<NetMessage> _tcpQueue;
    QQueue<NetMessage> _udpQueue;
};
