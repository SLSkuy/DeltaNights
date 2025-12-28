/* ------------------------------------------------------------
 *  Author:  2023051604044 wanrui
 *  Date:  2025.12.23
 *  LastUpdate: 2025.12.23
 *
 *  TCP封装头文件
 *
 *  功能简述：
 *  - 封装 QTcpServer / QTcpSocket
 *  - 提供统一的连接、断开、消息收发接口
 *  - 用于登录、房间、可靠控制指令
 * ------------------------------------------------------------ */

#pragma once

#include <QObject>
#include <QTcpServer>
#include <QTcpSocket>
#include <unordered_set>
#include <unordered_map>

class TcpEndpoint : public QObject
{
    Q_OBJECT
public:
    explicit TcpEndpoint(QObject* parent = nullptr);
    ~TcpEndpoint();

    bool listen(quint16 port, QHostAddress address = QHostAddress::Any);
    bool send(QTcpSocket* socket, const QByteArray& data);

signals:
    void clientConnected(QTcpSocket* socket);
    void clientDisconnected(QTcpSocket* socket);
    void messageReceived(QTcpSocket* socket, const QByteArray& data);

private slots:
    void onNewConnection();
    void onSocketReadyRead();
    void onSocketDisconnected();

private:
    QTcpServer* _server;
    std::unordered_set<QTcpSocket*> m_clients;

    std::unordered_map<QTcpSocket*, QByteArray> m_receiveBuffers;   // Socket缓存输入，黏包处理
};
