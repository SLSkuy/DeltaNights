#include <QCoreApplication>
#include "gameserver.h"
#include "test/server.h"

int main(int argc, char *argv[])
{
    QCoreApplication a(argc, argv);

    GameServer server;
    server.start(11451, 8888);


    return a.exec();
}
