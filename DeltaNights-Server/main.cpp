#include <QCoreApplication>
#include "gameserver.h"
#include "test/server.h"

int main(int argc, char *argv[])
{
    QCoreApplication a(argc, argv);

    //GameServer server;
    //server.start(11451, 8888);

    Server server;
    server.startServer(12345);

    return a.exec();
}
