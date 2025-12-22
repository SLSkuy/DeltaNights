/* ------------------------------------------------------------
 *  Author:  2023051604044 wanrui
 *  Date:  2025.12.22
 *  LastUpdate: 2025.12.22
 *
 *  Logger工具类头文件声明
 *
 *  功能简述：
 *  Logger用于格式化输出服务器日志
 *
 *  使用说明：
 *  - 与qDebug()使用方法一致
 * ------------------------------------------------------------ */

#pragma once

#include <QString>
#include <QTextStream>
#include <QDebug>
#include <QTime>

class LoggerStream
{
public:
    enum class Level
    {
        Info,
        Warning,
        Error
    };

    explicit LoggerStream(Level level);
    ~LoggerStream();

    template<typename T>
    LoggerStream& operator<<(const T& value)
    {
        stream << value;
        return *this;
    }

private:
    Level level;
    QString buffer;
    QTextStream stream;
};

class Logger
{
public:
    static LoggerStream Info();
    static LoggerStream Warning();
    static LoggerStream Error();
};
