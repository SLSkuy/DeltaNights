/* ------------------------------------------------------------
 *  Author:  2023051604044 wanrui
 *  Date:  2025.12.23
 *  LastUpdate: 2025.12.28
 *
 *  玩家信息管理类
 *  连接数据库，管理所有玩家信息
 * ------------------------------------------------------------ */

#include "playerinfomanager.h"
#include "playerinfo.h"
#include "../Logger/logger.h"

#include <QDebug>

PlayerInfoManager::PlayerInfoManager(QObject* parent)
    : QObject(parent)
{
    //测试用
    PlayerInfo *p=new PlayerInfo(123,"abc",this);
    p->setNickname("Test");
    p->setPassword("123");
    m_playerInfosByID[123]=p;
    m_playerInfosByAccount["abc"]=p;

}

PlayerInfoManager::~PlayerInfoManager()
{
    for(auto& it: m_playerInfosByID)
    {
        delete it.second;
    }
}

void PlayerInfoManager::logIn(QTcpSocket* socket,QString account, QString password)
{
    PlayerInfo* playerInfo=findPlayInfo(account);
    if(playerInfo){
        if(playerInfo->loginAccount(password)){
            emit clientBindPlayerInfo(socket,playerInfo);

            //发送确认
            using namespace SyncPackage;
            RemoteSyncPackage response;
            response.set_eventid(RemoteSyncEvent::ClientResponse);
            auto* type = response.mutable_clientpackage();
            type->set_eventid(ClientSyncPackage::RemoteClientEvent::LoginResponse);
            auto *loginResponse=type->mutable_loginresponse();
            loginResponse->set_uuid(playerInfo->uuid());
            loginResponse->set_nickname(playerInfo->nickname().toStdString());

            Logger::Info() <<"[PlayerInfoManager]"<<playerInfo->uuid()<<"-"<<playerInfo->nickname();
            emit clientLoginResponse(socket,response);
        }
        else{

        }

    }else{
       //返回空
    }
}

PlayerInfo* PlayerInfoManager::findPlayInfo(QString account)
{
    auto p=m_playerInfosByAccount.find(account);
    if(p!=m_playerInfosByAccount.end()){
        return p->second;
    }else{
        return nullptr;
    }
}
