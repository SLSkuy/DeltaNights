#pragma once

#include <QObject>

class MsgSender : public QObject
{
    Q_OBJECT
public:
    MsgSender();
    void SendMessage();
signals:
    void send(QString content);
    void workFinished();
};

