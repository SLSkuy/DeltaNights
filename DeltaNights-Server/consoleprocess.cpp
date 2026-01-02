#include "consoleprocess.h"
#include <iostream>

ConsoleProcess::ConsoleProcess(QObject* parent)
    : QThread(parent)
{
}

void ConsoleProcess::run()
{
    std::string line;

    while (!isInterruptionRequested())
    {
        if (!std::getline(std::cin, line))
        {
            break;
        }

        if (!line.empty())
        {
            emit commandReceived(QString::fromStdString(line));

            // 结束命令捕获
            if(line == "stop")
            {
                break;
            }
        }
    }
}
