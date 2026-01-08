/* ------------------------------------------------------------
*  Author:  2023051604044 wenrenqiang
 *  Date:  2026.1.6
 *  LastUpdate: 2026.1.6
 *
 *  功能：
 *  - 账号数据读取，暂时用文本处理
 * ------------------------------------------------------------ */
#ifndef DATAREADING_H
#define DATAREADING_H

#include <QFile>
#include <QTextStream>
#include <QFileInfo>
#include <QDir>
#include "../DeltaNights-Server/ClientManage/playerinfo.h"
#include <unordered_map>
#include <QObject>

class DataLoad : QObject
{
public:
    DataLoad(QObject* parent = nullptr);
    void read();
    void write();//初始创建文本文件，一般不使用
    void appendToFile();//追加写入
    void loading();//读取数据
    std::unordered_map<quint32, PlayerInfo*> playerInfosByID(){return m_playerInfosByID;};
    std::unordered_map<QString, PlayerInfo*> playerInfosByAccount(){return m_playerInfosByAccount;};

private:
    PlayerInfo * pl;
    std::unordered_map<quint32, PlayerInfo*> m_playerInfosByID;//主索引 uuid
    std::unordered_map<QString, PlayerInfo*> m_playerInfosByAccount;//用户名索引
};

#endif // DATAREADING_H
