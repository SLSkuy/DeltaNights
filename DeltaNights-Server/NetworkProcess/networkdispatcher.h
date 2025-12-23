/* ------------------------------------------------------------
 *  Author:  2023051604044 wanrui
 *  Date:  2025.12.23
 *  LastUpdate: 2025.12.23
 *
 *  网络分发器
 *  处理UDP、TCP的数据收发
 *  负责Protobuf事件的序列化与反序列化
 * ------------------------------------------------------------ */

#pragma once

#include <QObject>

class UdpEndpoint;
class TcpEndpoint;
class GameRoom;

class NetworkDispatcher : public QObject
{
    Q_OBJECT
public:
    explicit NetworkDispatcher(UdpEndpoint* udp, TcpEndpoint* tcp, QObject* parent = nullptr);

    void onTcpMessage();    // 处理TCP接受的消息
    void onUdpMessage();    // 处理UDP接受的消息

    void broadcastRoomFrame(GameRoom* room);    // 广播战局事件

signals:
    // 发送各种事件信号
    void loginRequest();

private:
    UdpEndpoint* _udp;
    TcpEndpoint* _tcp;
};
