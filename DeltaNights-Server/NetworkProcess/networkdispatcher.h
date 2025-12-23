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
class GameRoom;

class NetworkDispatcher : public QObject
{
    Q_OBJECT
public:
    explicit NetworkDispatcher(UdpEndpoint* udp, QObject* parent = nullptr);

    void broadcastRoomFrame(GameRoom* room);    // 广播战局事件

private:
    UdpEndpoint* m_udp;
};
