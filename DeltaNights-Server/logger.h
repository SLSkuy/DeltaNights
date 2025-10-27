#pragma once

#include <QString>
#include <QTcpSocket>

class Logger
{
public:
    static void Log(const QString &content);
    static void Error(const QString &content);
    static void Warning(const QString &content);
    static void Message(const QString &content, QTcpSocket *socket);
};
