/* ------------------------------------------------------------
 *  Author:  2023051604044 wanrui
 *  Date:  2025.12.23
 *  LastUpdate: 2025.12.28
 *
 *  战局内玩家抽象
 *  计算玩家状态并同步给客户端
 * ------------------------------------------------------------ */

#include "playerentity.h"
#include "../PlayerInfo/playerinfo.h"

PlayerEntity::PlayerEntity(PlayerInfo* info)
    : m_uuid(info->uuid())
    , m_nickname(info->nickname())
{

}

void PlayerEntity::bindClient(ClientInfo* client)
{
    m_client = client;
}

void PlayerEntity::tick()
{
    // TODO: 根据输入计算期望速度

    // TODO: 跳跃处理

    // TODO: 朝向更新
}
