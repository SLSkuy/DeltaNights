/* ------------------------------------------------------------
 *  Author:  2023051604044 wanrui
 *  Date:  2025.12.22
 *  LastUpdate: 2025.12.30
 *
 *  UDP封装实现
 *
 *  功能简述：
 *  初步封装UDP功能
 * ------------------------------------------------------------ */

#include <QDebug>

#include "UdpEndpoint.h"
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

    bool ok = _socket->bind(address, port, QUdpSocket::ShareAddress | QUdpSocket::ReuseAddressHint);

    if (!ok)
    {
        Logger::Warning() << "UDP bind failed:" << _socket->errorString();
    }
    else
    {
        Logger::Info() << "UDP bind on " << address.toString() << ":" << port;
    }
    return ok;
}

bool UdpEndpoint::send(const QByteArray& data, const QHostAddress& address, quint16 port)
{
    if (data.isEmpty())
        return false;

    qint64 sent = _socket->writeDatagram(data, address, port);
    return sent == data.size();
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

        // 测试消息
        Logger::Info() << "Receive Message: " << QString::fromUtf8(datagram)
                       << " from: " << sender.toString()
                       << ":" << senderPort;
        send(QString("服务器收到UDP连接").toUtf8(), sender, senderPort);

        emit messageReceived(sender, senderPort, datagram);
    }
}
