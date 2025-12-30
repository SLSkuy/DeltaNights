/* ------------------------------------------------------------
 *  Author:  2023051604044 wanrui
 *  Date:  2025.12.23
 *  LastUpdate: 2025.12.30
 *
 *  网络分发器
 *  处理UDP、TCP的数据收发
 *  负责Protobuf事件的序列化与反序列化
 * ------------------------------------------------------------ */

#include "networkdispatcher.h"
#include "tcpendpoint.h"
#include "udpendpoint.h"
#include "../Logger/logger.h"
#include "../SyncPackage.pb.h"

NetworkDispatcher::NetworkDispatcher(UdpEndpoint* udp, TcpEndpoint* tcp, QObject* parent)
    : _udp(udp), _tcp(tcp)
    , QObject(parent)
{
    // TCP信号
    connect(_tcp, &TcpEndpoint::messageReceived, this, &NetworkDispatcher::onTcpMessage, Qt::QueuedConnection);
    connect(_tcp, &TcpEndpoint::clientConnected, this, &NetworkDispatcher::clientConnect, Qt::QueuedConnection);

    // UDP信号
    connect(_udp, &UdpEndpoint::messageReceived, this, &NetworkDispatcher::onUdpMessage, Qt::QueuedConnection);
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
        const NetMessage& msg = tcpMsgs.dequeue();
        handleTcpPackage(msg.socket, msg.data);
    }

    // ===== 处理 UDP 消息 =====
    while (!udpMsgs.isEmpty())
    {
        // TODO: Protobuf 反序列化
        // TODO: 玩家输入 / 位移 / 朝向
    }
}

void NetworkDispatcher::handleTcpPackage(QTcpSocket* socket, const QByteArray& data)
{
    using namespace SyncPackage;

    LocalSyncPackage pkg;
    if (!pkg.ParseFromArray(data.constData(), data.size()))
    {
        Logger::Warning() << "[NetworkDispatcher] TCP protobuf parse failed"
                   << "size = " << data.size();
        return;
    }

    // ===== 按类型分发 =====
    switch (pkg.eventid())
    {
        case LocalSyncEvent::Ack:
            handleTcpAckPackage(socket,pkg.acksync());
            break;

        default:
            Logger::Warning() << "[NetworkDispatcher] Unknown TCP package type:" << pkg.eventid();
            break;
    }
}

void NetworkDispatcher::handleTcpAckPackage(QTcpSocket* socket, const AckPackage::AckSyncRequest& pkg)
{
    using namespace AckPackage;

    switch (pkg.eventid())
    {
        case AckSyncEvent::HeartBeat:
            // TODO: 心跳消息处理
            emit clientHeartBeat(socket);
            break;
        case AckSyncEvent::Connect:
            // TODO: 客户端连接请求
            emit clientBindUdpPort(socket, pkg.connect().port());
            break;
        default:
            Logger::Warning() << "[NetworkDispatcher] Unknown TCP_ACK package type:" << pkg.eventid();
            break;
    }
}
