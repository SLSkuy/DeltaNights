#include "logger.h"

#include <QDebug>
#include <QTime>

void Logger::Log(const QString &content)
{
    qDebug().noquote() << "[INFO" << QTime::currentTime().toString("HH:mm:ss") + "] " + content;
}

void Logger::Error(const QString &content)
{
    qDebug().noquote() << "[ERROR" << QTime::currentTime().toString("HH:mm:ss") + "] " + content;
}

void Logger::Warning(const QString &content)
{
    qDebug().noquote() << "[WARNING" << QTime::currentTime().toString("HH:mm:ss") + "] " + content;
}

void Logger::Message(const QString &content, QTcpSocket *socket)
{
    qDebug().noquote() << "[Message from" << socket->localPort()
    << QTime::currentTime().toString("HH:mm:ss") + "] " + content;
}
