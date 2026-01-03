#include "playerinfo.h"

PlayerInfo::PlayerInfo(quint32 uuid, QObject* parent)
    : m_uuid(uuid), QObject(parent)
{

}

PlayerInfo::PlayerInfo(quint32 uuid, QString account, QObject *parent)
    : m_uuid(uuid),m_account(account), QObject(parent)
{

}

bool PlayerInfo::loginAccount(QString password)
{
    return password==m_password;
}

void PlayerInfo::setNickname(QString nickname)
{
    m_nickname=nickname;
}
void PlayerInfo::setPassword(QString password)
{
    m_password=password;
}
