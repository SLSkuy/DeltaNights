#include <iostream>

#include "msgsender.h"
#include "logger.h"

MsgSender::MsgSender() {}

void MsgSender::SendMessage()
{
    std::string input;
    Logger::Log("Input exit or quit to close server");
    while (true)
    {
        std::getline(std::cin, input);

        if (input == "exit" || input == "quit") {
            emit workFinished();
            break;
        }else if(input == "")
        {
            Logger::Warning("Unknown command");
            continue;
        }

        QString msg = QString::fromStdString(input);
        emit send(msg);
    }
}
