/* ------------------------------------------------------------
 *  Author:  2023051604044 wanrui
 *  Date:  2025.12.23
 *  LastUpdate: 2025.12.23
 *
 *  玩家信息管理类
 *  连接数据库，管理所有玩家信息
 * ------------------------------------------------------------ */

#pragma once

#include <QObject>
#include <unordered_map>

class PlayerInfo;

class PlayerInfoManager : public QObject
{
    Q_OBJECT
public:
    explicit PlayerInfoManager(QObject *parent = nullptr);

private:
    std::unordered_map<QString, PlayerInfo*> m_playerInfos;
};
