#include <QCoreApplication>
#include "gameserver.h"

int main(int argc, char *argv[])
{
    QCoreApplication a(argc, argv);

    GameServer server;
    server.start(11451, 19198);

    return a.exec();
}
