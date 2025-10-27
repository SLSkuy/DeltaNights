#include <QCoreApplication>
#include "dnserver.h"

int main(int argc, char *argv[])
{
    QCoreApplication a(argc, argv);

    DNServer server;
    QObject::connect(&server, &DNServer::closeServer, &a, &QCoreApplication::quit);
    if (!server.StartServer(QHostAddress::Any, 11451)) {
        return -1;
    }

    return a.exec();
}
