/* ------------------------------------------------------------
 *  Author:  2023051604044 wanrui
 *  Date:  2025.12.23
 *  LastUpdate: 2025.12.31
 *
 *  网络分发器
 *  处理UDP、TCP的数据收发
 *  负责Protobuf事件的序列化与反序列化
 * ------------------------------------------------------------ */

#include <QtEndian>

#include "networkdispatcher.h"
#include "tcpendpoint.h"
#include "udpendpoint.h"
#include "../Logger/logger.h"
#include "../GameEvent/SyncPackage.pb.h"

NetworkDispatcher::NetworkDispatcher(UdpEndpoint* udp, TcpEndpoint* tcp, QObject* parent)
    : _udp(udp), _tcp(tcp)
    , QObject(parent)
{
    // TCP信号
    connect(_tcp, &TcpEndpoint::messageReceived, this, &NetworkDispatcher::onTcpMessage, Qt::QueuedConnection);
    connect(_tcp, &TcpEndpoint::clientConnected, this, &NetworkDispatcher::clientConnect, Qt::QueuedConnection);
    connect(this, &NetworkDispatcher::sendTcp, _tcp, &TcpEndpoint::send, Qt::QueuedConnection);

    // UDP信号
    connect(_udp, &UdpEndpoint::messageReceived, this, &NetworkDispatcher::onUdpMessage, Qt::QueuedConnection);
    connect(this, &NetworkDispatcher::sendUdp, _udp, &UdpEndpoint::send, Qt::QueuedConnection);
}

/* ============================================================
 * 接收消息处理
============================================================ */
void NetworkDispatcher::onTcpMessage(QTcpSocket* socket, const QByteArray& data)
{
    // TODO: 加入TCP消息队列
    QMutexLocker locker(&m_mutex);
    m_tcpQueue.enqueue(NetMessage{socket, QHostAddress(), 0, data});
}

void NetworkDispatcher::onUdpMessage(const QHostAddress& addr, quint16 port, const QByteArray& data)
{
    // TODO: 加入UDP消息队列
    QMutexLocker locker(&m_mutex);
    m_udpQueue.enqueue(NetMessage{nullptr, addr, port, data});
}

void NetworkDispatcher::processQueueMessage()
{
    QQueue<NetMessage> tcpMsgs;
    QQueue<NetMessage> udpMsgs;

    QMutexLocker locker(&m_mutex);
    tcpMsgs.swap(m_tcpQueue);
    udpMsgs.swap(m_udpQueue);

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
        const NetMessage& msg = udpMsgs.dequeue();
        handleUdpPackage(msg.addr, msg.port, msg.data);
    }
}

/* ============================================================
 * 发送消息处理
============================================================ */
void NetworkDispatcher::sendTcpMessage(QTcpSocket* socket,const SyncPackage::RemoteSyncPackage& pkg)
{
    if (!socket) return;

    QByteArray body;
    body.resize(pkg.ByteSizeLong());

    if (!pkg.SerializeToArray(body.data(), body.size()))
        return;

    // 写入头部消息
    QByteArray packet;
    qint32 bodyLen = body.size();
    qint32 headLen = qToBigEndian(bodyLen);

    packet.append(reinterpret_cast<const char*>(&headLen), sizeof(qint32));
    packet.append(body);

    emit sendTcp(socket, packet);
}

void NetworkDispatcher::sendUdpMessage(const QHostAddress& addr, quint16 port, const SyncPackage::RemoteSyncPackage& pkg)
{
    QByteArray datagram;
    datagram.resize(pkg.ByteSizeLong());

    if (!pkg.SerializeToArray(datagram.data(), datagram.size()))
        return;

    emit sendUdp(addr, port, datagram);
}

/* ============================================================
 * TCP消息处理
============================================================ */
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
        case LocalSyncEvent::AckRequest:
            // TODO: 处理ACK同步请求
            handleTcpAckPackage(socket,pkg.acksync());
            break;
        case LocalSyncEvent::LobbyRequest:
            // TODO: 处理大厅操作请求
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
        case LocalAckEvent::HeartBeat:
            // TODO: 心跳消息处理
            emit clientHeartBeat(socket);
            break;
        case LocalAckEvent::Connect:
            // TODO: 客户端连接请求
            emit clientBindUdpPort(socket, pkg.connect().port());
            break;
        default:
            Logger::Warning() << "[NetworkDispatcher] Unknown TCP_ACK package type:" << pkg.eventid();
            break;
    }
}

/* ============================================================
 * UDP消息处理
============================================================ */
void NetworkDispatcher::handleUdpPackage(const QHostAddress& addr, quint16 port, const QByteArray& data)
{
    using namespace SyncPackage;

    LocalSyncPackage pkg;
    if (!pkg.ParseFromArray(data.constData(), data.size()))
    {
        Logger::Warning() << "[NetworkDispatcher] UDP protobuf parse failed" << "size = " << data.size();
        return;
    }

    // ===== 按类型分发 =====
    switch (pkg.eventid())
    {
        case LocalSyncEvent::AckRequest:
            // TODO: 处理ACK类同步包
            break;
        case LocalSyncEvent::BattleRequest:
            // TODO: 处理战局同步请求
            break;
        default:
            Logger::Warning() << "[NetworkDispatcher] Unknown UDP package type:" << pkg.eventid();
            break;
    }
}
