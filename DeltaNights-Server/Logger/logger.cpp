/* ------------------------------------------------------------
 *  Author:  2023051604044 wanrui
 *  Date:  2025.10.28
 *  LastUpdate: 2025.12.22
 *
 *  Logger工具类实现
 *
 *  功能简述：
 *  Logger用于格式化输出服务器日志
 *
 *  使用说明：
 *  - 与qDebug()使用方法一致
 * ------------------------------------------------------------ */

#include "logger.h"

LoggerStream::LoggerStream(Level level)
    : level(level), stream(&buffer)
{
}

LoggerStream::~LoggerStream()
{
    QString prefix;
    switch (level)
    {
    case Level::Info:
        prefix = "[INFO ";
        break;
    case Level::Warning:
        prefix = "[WARNING ";
        break;
    case Level::Error:
        prefix = "[ERROR ";
        break;
    }

    QString time = QTime::currentTime().toString("HH:mm:ss");
    qDebug().noquote() << prefix + time + "] " + buffer;
}

LoggerStream Logger::Info()
{
    return LoggerStream(LoggerStream::Level::Info);
}

LoggerStream Logger::Warning()
{
    return LoggerStream(LoggerStream::Level::Warning);
}

LoggerStream Logger::Error()
{
    return LoggerStream(LoggerStream::Level::Error);
}
