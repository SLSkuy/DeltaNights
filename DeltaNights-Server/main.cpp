#include <QCoreApplication>
#include <QTimer>
#include "logger.h"
#include "UdpEndpoint.h"

int main(int argc, char *argv[])
{
    QCoreApplication a(argc, argv);

    UdpEndpoint udp;

    udp.bind(7777, QHostAddress::Any);

    QObject::connect(&udp, &UdpEndpoint::messageReceived,[](const QByteArray& data, const QHostAddress& from, quint16 port)
    {
        Logger::Info() << "Recv: " << data << " from " << from.toString() << ":" << port;
    });

    // UDP测试
    QTimer::singleShot(1000, [&udp]()
    {
        udp.send("UDP test", QHostAddress::LocalHost, 7777);
    });

    return a.exec();
}
