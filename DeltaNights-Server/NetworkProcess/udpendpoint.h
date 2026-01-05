/* ------------------------------------------------------------
 *  Author:  2023051604044 wanrui
 *  Date:  2025.12.22
 *  LastUpdate: 2025.12.31
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
#include <QQueue>
#include <QMutex>
#include <QTimer>

struct UdpMessage
{
    QHostAddress addr;
    quint16 port;
    QByteArray data;
};

class UdpEndpoint : public QObject
{
    Q_OBJECT
public:
    explicit UdpEndpoint(QObject* parent = nullptr);
    ~UdpEndpoint();

    bool bind(quint16 port, QHostAddress address = QHostAddress::Any);
    void send(const QHostAddress& address, quint16 port, const QByteArray& data);

signals:
    void messageReceived(const QHostAddress& from, quint16 port, const QByteArray& data);

private:
    void onReadyRead();
    void processSendQueue();

private:
    QUdpSocket* _socket = nullptr;

    QQueue<UdpMessage> m_sendQueue;
    QMutex m_sendMutex;
    QTimer* _sendTimer = nullptr;
    int m_udpRate = 128; // Hz
};
