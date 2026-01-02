#pragma once

#include <QThread>
#include <QString>

class ConsoleProcess : public QThread
{
    Q_OBJECT
public:
    explicit ConsoleProcess(QObject* parent = nullptr);

signals:
    void commandReceived(const QString& cmd);

protected:
    void run() override;
};
