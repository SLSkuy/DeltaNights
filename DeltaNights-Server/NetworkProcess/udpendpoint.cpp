/* ------------------------------------------------------------
 *  Author:  2023051604044 wanrui
 *  Date:  2025.12.22
 *  LastUpdate: 2025.12.31
 *
 *  UDP封装实现
 *
 *  功能简述：
 *  初步封装UDP功能
 * ------------------------------------------------------------ */

#include <QDebug>

#include "udpendpoint.h"
#include "../Logger/logger.h"

UdpEndpoint::UdpEndpoint(QObject* parent)
    : QObject(parent)
{
}

UdpEndpoint::~UdpEndpoint()
{
    if(_socket)
    {
        _socket->close();
    }
}

bool UdpEndpoint::bind(quint16 port, QHostAddress address)
{
    // 延迟创建Socket，不在构造时创建，导致对象线程依赖出错
    if(!_socket)
    {
        _socket = new QUdpSocket(this);
        connect(_socket, &QUdpSocket::readyRead, this, &UdpEndpoint::onReadyRead);
    }

    if (_socket->state() == QUdpSocket::BoundState)
        _socket->close();

    if(!_sendTimer)
    {
        _sendTimer = new QTimer(this);
        connect(_sendTimer, &QTimer::timeout, this, &UdpEndpoint::processSendQueue);
        _sendTimer->start(1000 / m_udpRate);
        Logger::Info() << "[TcpEndPoint]: UDP send Rate " << m_udpRate << " Hz";
    }

    bool ok = _socket->bind(address, port, QUdpSocket::ShareAddress | QUdpSocket::ReuseAddressHint);

    if (!ok)
    {
        Logger::Warning() << "[UdpEndpoint]: UDP bind failed    :" << _socket->errorString();
    }
    else
    {
        Logger::Info() << "[UdpEndpoint]: UDP bind on " << address.toString() << ":" << port;
    }
    return ok;
}

void UdpEndpoint::send(const QHostAddress& address, quint16 port, const QByteArray& data)
{
    if (data.isEmpty())
        return;

    QMutexLocker lock(&m_sendMutex);
    m_sendQueue.enqueue({address, port, data});
}

void UdpEndpoint::onReadyRead()
{
    while (_socket->hasPendingDatagrams())
    {
        QByteArray datagram;
        datagram.resize(int(_socket->pendingDatagramSize()));

        QHostAddress sender;
        quint16 senderPort;

        _socket->readDatagram(datagram.data(), datagram.size(), &sender, &senderPort);

        emit messageReceived(sender, senderPort, datagram);
    }
}

void UdpEndpoint::processSendQueue()
{
    QQueue<UdpMessage> udpMsgs;

    QMutexLocker lock(&m_sendMutex);
    udpMsgs.swap(m_sendQueue);
    while(!udpMsgs.isEmpty())
    {
        auto it = udpMsgs.dequeue();

        _socket->writeDatagram(it.data, it.addr, it.port);
    }
}
