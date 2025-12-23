/* ------------------------------------------------------------
 *  Author:  2023051604044 wanrui
 *  Date:  2025.12.23
 *  LastUpdate: 2025.12.23
 *
 *  网络分发器
 *  处理UDP、TCP的数据收发
 *  负责Protobuf事件的序列化与反序列化
 * ------------------------------------------------------------ */

#include "networkdispatcher.h"

NetworkDispatcher::NetworkDispatcher(UdpEndpoint* udp, QObject* parent)
    : m_udp(udp)
    , QObject(parent)
{
}

void NetworkDispatcher::broadcastRoomFrame(GameRoom* room)
{
    // TODO: 广播某一个战局内的同步事件
}
