#include <QCoreApplication>
#include "gameserver.h"
#include "consoleprocess.h"

int main(int argc, char *argv[])
{
    QCoreApplication app(argc, argv);

    GameServer* server = new GameServer;
    server->start(11451, 19198);

    ConsoleProcess* console = new ConsoleProcess;

    QObject::connect(console, &ConsoleProcess::commandReceived, server, &GameServer::handleConsoleCommand, Qt::QueuedConnection);
    QObject::connect(&app, &QCoreApplication::aboutToQuit, [&]() {
        console->requestInterruption();
        console->wait();
        console->deleteLater();
        server->deleteLater();
    });

    console->start();

    return app.exec();
}
