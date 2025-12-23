/* ------------------------------------------------------------
 *  Author:  2023051604044 wanrui
 *  Date:  2025.12.22
 *  LastUpdate: 2025.12.22
 *
 *  UDP封装头文件
 *
 *  功能简述：
 *  初步封装UDP功能
 * ------------------------------------------------------------ */

#pragma once

#include <QObject>
#include <QUdpSocket>
#include <QHostAddress>

class UdpEndpoint : public QObject
{
    Q_OBJECT

public:
    explicit UdpEndpoint(QObject* parent = nullptr);
    ~UdpEndpoint();

    bool bind(quint16 port, QHostAddress address = QHostAddress::AnyIPv4);
    bool send(const QByteArray& data, const QHostAddress& address, quint16 port);

signals:
    void messageReceived(const QByteArray& data, const QHostAddress& from, quint16 port);

private slots:
    void onReadyRead();

private:
    QUdpSocket* _socket;
};
