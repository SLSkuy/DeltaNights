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

PlayerInfoManager::PlayerInfoManager(QObject* parent)
    : QObject(parent)
{
}

PlayerInfoManager::~PlayerInfoManager()
{
    for(auto& it: m_playerInfos)
    {
        delete it.second;
    }
}
