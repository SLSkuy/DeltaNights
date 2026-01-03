/* ------------------------------------------------------------
 *  Author:  2023051604044 wanrui
 *  Date:  2025.12.23
 *  LastUpdate: 2026.1.2
 *
 *  战局内玩家抽象
 *  计算玩家状态并同步给客户端
 * ------------------------------------------------------------ */

#include "playerentity.h"
#include "../ClientManage/playerinfo.h"

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
    // TODO: 生成期望位置
}

void PlayerEntity::updateInput(BattleSyncPackage::BattleSyncRequest* input)
{
    // 更新玩家输入
    m_input.moveDir = input->movedir();
    m_input.yaw = input->yaw();
    m_input.pitch = input->pitch();
    m_input.jump = input->jump();
    m_input.activeSkill = input->activeskill();
    m_input.ultimateSkill = input->ultimateskill();

    // 测试使用，直接计算权威状态
    m_position = input->position();
    m_eulaAngle = input->eulaangle();
}
