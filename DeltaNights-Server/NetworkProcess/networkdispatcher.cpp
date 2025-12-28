/* ------------------------------------------------------------
 *  Author:  2023051604044 wanrui
 *  Date:  2025.12.23
 *  LastUpdate: 2025.12.28
 *
 *  网络分发器
 *  处理UDP、TCP的数据收发
 *  负责Protobuf事件的序列化与反序列化
 * ------------------------------------------------------------ */

#include "networkdispatcher.h"
#include "tcpendpoint.h"
#include "udpendpoint.h"

NetworkDispatcher::NetworkDispatcher(UdpEndpoint* udp, TcpEndpoint* tcp, QObject* parent)
    : _udp(udp), _tcp(tcp)
    , QObject(parent)
{
}

void NetworkDispatcher::broadcastRoomFrame(GameRoom* room)
{
    // TODO: 广播某一个战局内的同步事件
}

void NetworkDispatcher::onTcpMessage(QTcpSocket* socket, const QByteArray& data)
{
    // TODO: 加入TCP消息队列
    QMutexLocker locker(&_mutex);
    _tcpQueue.enqueue(NetMessage{socket, QHostAddress(), 0, data});
}

void NetworkDispatcher::onUdpMessage(const QHostAddress& addr, quint16 port, const QByteArray& data)
{
    // TODO: 加入UDP消息队列
    QMutexLocker locker(&_mutex);
    _udpQueue.enqueue(NetMessage{nullptr, addr, port, data});
}

void NetworkDispatcher::processQueueMessage()
{
    QQueue<NetMessage> tcpMsgs;
    QQueue<NetMessage> udpMsgs;

    QMutexLocker locker(&_mutex);
    tcpMsgs.swap(_tcpQueue);
    udpMsgs.swap(_udpQueue);

    // ===== 处理 TCP 消息 =====
    while (!tcpMsgs.isEmpty())
    {
        // TODO: Protobuf 反序列化
        // TODO: 登录 / 房间 / 控制指令
    }

    // ===== 处理 UDP 消息 =====
    while (!udpMsgs.isEmpty())
    {
        // TODO: Protobuf 反序列化
        // TODO: 玩家输入 / 位移 / 朝向
    }
}

