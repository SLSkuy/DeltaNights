/* ------------------------------------------------------------
 *  Author:  2023051604044 wanrui
 *  Date:  2025.12.23
 *  LastUpdate: 2026.1.2
 *
 *  玩家信息类
 *  记录玩家如账号、密码、昵称、货币等各种信息
 *  不同于PlayerEntity作为战局内实例
 *  PlayerInfo持续存在于数据库中，并在客户端登录后，绑定客户端
 * ------------------------------------------------------------ */

#pragma once

#include <QObject>

#include "../ClientManage/clientinfo.h"

class PlayerInfo : public QObject
{
    Q_OBJECT
public:
    explicit PlayerInfo(quint32 uuid, QObject* parent = nullptr);
    explicit PlayerInfo(quint32 uuid,QString account, QObject* parent = nullptr);
    void registerAccount();
    bool loginAccount(QString password);
    void setClientID(quint32 clientId);
    void setNickname(QString nickname);
    void setPassword(QString password);

public:
    quint32 uuid() const {return m_uuid;}
    QString nickname() const {return m_nickname;}
    quint32 getClientID() const {return m_clientID;}

private:
    quint32 m_uuid;
    QString m_nickname;

    quint32 m_clientID;

    QString m_account;
    QString m_password;
};
