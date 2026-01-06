/* ------------------------------------------------------------
 *  Author:  2023051604044 wanrui
 *           2023051604046 wenrenqiang
 *  Date:  2025.12.23
 *  LastUpdate: 2026.1.6
 *
 *  玩家信息管理类
 *  连接数据库，管理所有玩家信息
 * ------------------------------------------------------------ */

#pragma once

#include <QObject>
#include <unordered_map>
#include <QTcpSocket>
#include "../GameEvent/SyncPackage.pb.h"
#include "../dataload.h"

class PlayerInfo;

class PlayerInfoManager : public QObject
{
    Q_OBJECT
public:
    explicit PlayerInfoManager(QObject *parent = nullptr);
    ~PlayerInfoManager();

    void logIn(QTcpSocket* socket,QString account,QString password);
    PlayerInfo* findPlayInfo(QString account);
    void loadData();
signals:
    void clientBindPlayerInfo(QTcpSocket* socket,PlayerInfo *playerInfo);
    void clientLoginResponse(QTcpSocket* socket, const SyncPackage::RemoteSyncPackage& pkg);

private:
    std::unordered_map<quint32, PlayerInfo*> m_playerInfosByID;//主索引 uuid
    std::unordered_map<QString, PlayerInfo*> m_playerInfosByAccount;//用户名索引
    DataLoad* DataLoading;//玩家账号数据读取
};
