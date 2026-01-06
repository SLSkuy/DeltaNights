/* ------------------------------------------------------------
 *  Author:  2023051604044 wanrui
 *           2023051604046 wenrenqiang
 *  Date:  2025.12.23
 *  LastUpdate: 2026.1.6
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
#include "../ObjectPool/protopool.h"

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

void NetworkDispatcher::sendUdpMessage(const QHostAddress& addr, quint16 port, BattleSyncPackage::BattleSyncResponse* pkg)
{
    QByteArray datagram;
    datagram.resize(pkg->ByteSizeLong());

    if (!pkg->SerializeToArray(datagram.data(), datagram.size()))
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
        case LocalSyncEvent::ClientRequest:
            handleTcpClientPackage(socket, pkg.clientsync());
            break;
        case LocalSyncEvent::LobbyRequest:
            handleTcpLobbyPackage(socket, pkg.lobbysync());
            break;
        default:
            Logger::Warning() << "[NetworkDispatcher] Unknown TCP package type:" << pkg.eventid();
            break;
    }
}

void NetworkDispatcher::handleTcpClientPackage(QTcpSocket* socket, const ClientSyncPackage::ClientSyncRequest& pkg)
{
    using namespace ClientSyncPackage;

    // ===== 按子类型分发 =====
    switch (pkg.eventid())
    {
        case LocalClientEvent::HeartBeat:
            // TODO: 心跳消息处理
            emit clientHeartBeat(socket);
            break;
        case LocalClientEvent::ConnectRequest:
            // TODO: 客户端连接请求
            emit clientBindUdpPort(socket, pkg.connect().port());
            break;
        case LocalClientEvent::LoginRequest:
            // TODO: 客户端登录请求
            Logger::Info()<<"[NetworkDispatcher]"<<QString::fromStdString(pkg.loginrequest().account())<<"-"<<QString::fromStdString(pkg.loginrequest().password());
            emit clientLogin(socket,QString::fromStdString(pkg.loginrequest().account()),QString::fromStdString(pkg.loginrequest().password()));
            break;
        default:
            Logger::Warning() << "[NetworkDispatcher] Unknown TCP_ACK package type:" << pkg.eventid();
            break;
    }
}

void NetworkDispatcher::handleTcpLobbyPackage(QTcpSocket* socket, const LobbySyncPackage::LobbySyncRequest& pkg)
{
    using namespace LobbySyncPackage;

    // ===== 按子类型分发 =====
    switch (pkg.eventid())
    {
        //case
        default:
            Logger::Warning() << "[NetworkDispatcher] Unknown TCP_Lobby package type:" << pkg.eventid();
            break;
    }
}

/* ============================================================
 * UDP消息处理
============================================================ */
void NetworkDispatcher::handleUdpPackage(const QHostAddress& addr, quint16 port, const QByteArray& data)
{
    // TODO: UDP只处理战局同步事件和ACK包
    using namespace BattleSyncPackage;

    BattleSyncRequest* pkg = ProtoPool::AcquireBattleReq();
    if(!pkg->ParseFromArray(data.constData(), data.size()))
    {
        Logger::Warning() << "[NetworkDispatcher] UDP protobuf parse failed"
                          << "size = " << data.size();
        return;
    }

    emit battleSyncRequest(pkg);
    ProtoPool::Release(pkg);
}
