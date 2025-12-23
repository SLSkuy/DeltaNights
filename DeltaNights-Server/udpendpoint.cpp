/* ------------------------------------------------------------
 *  Author:  2023051604044 wanrui
 *  Date:  2025.12.22
 *  LastUpdate: 2025.12.22
 *
 *  UDP封装实现
 *
 *  功能简述：
 *  初步封装UDP功能
 * ------------------------------------------------------------ */

#include "UdpEndpoint.h"
#include <QDebug>

UdpEndpoint::UdpEndpoint(QObject* parent)
    : QObject(parent)
{
    connect(&_socket, &QUdpSocket::readyRead, this, &UdpEndpoint::onReadyRead);
}

UdpEndpoint::~UdpEndpoint()
{
    _socket.close();
}

bool UdpEndpoint::bind(quint16 port, QHostAddress address)
{
    if (_socket.state() == QUdpSocket::BoundState)
        _socket.close();

    bool ok = _socket.bind(address, port, QUdpSocket::ShareAddress | QUdpSocket::ReuseAddressHint);

    if (!ok)
    {
        qWarning() << "UDP bind failed:" << _socket.errorString();
    }
    else
    {
        qDebug() << "UDP bind on" << address.toString() << port;
    }
    return ok;
}

bool UdpEndpoint::send(const QByteArray& data, const QHostAddress& address, quint16 port)
{
    if (data.isEmpty())
        return false;

    qint64 sent = _socket.writeDatagram(data, address, port);
    return sent == data.size();
}

void UdpEndpoint::onReadyRead()
{
    while (_socket.hasPendingDatagrams())
    {
        QByteArray datagram;
        datagram.resize(int(_socket.pendingDatagramSize()));

        QHostAddress sender;
        quint16 senderPort;

        _socket.readDatagram(datagram.data(), datagram.size(), &sender, &senderPort);

        emit messageReceived(datagram, sender, senderPort);
    }
}
