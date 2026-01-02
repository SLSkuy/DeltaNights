/* ------------------------------------------------------------
 *  Author:  2023051604044 wanrui
 *  Date:  2025.12.23
 *  LastUpdate: 2026.1.2
 *
 *  战局内玩家抽象实体
 *  记录客户端输入
 *  计算玩家状态并同步给客户端
 * ------------------------------------------------------------ */

#pragma once

#include <memory.h>

#include "../GameEvent/UnityMath.pb.h"
#include "../GameData/characterprops.h"
#include "../GameEvent/BattleSyncPackage.pb.h"

struct PlayerInput
{
    UnityMath::Vector2D moveDir;
    bool jump = false;
    bool fire = false;

    float yaw = 0.0f;
    float pitch = 0.0f;

    bool activeSkill = false;
    bool ultimateSkill = false;
};

class ClientInfo;
class PlayerInfo;

class PlayerEntity
{
public:
    explicit PlayerEntity(PlayerInfo* info);

    void tick();    // 逻辑更新，生成期望位移等交给碰撞系统处理
    void updateInput(BattleSyncPackage::BattleSyncRequest* input);

    void bindClient(ClientInfo* client);

public:
    quint32 uuid() const {return m_uuid;}
    QString nickname() const {return m_nickname;}
    PlayerInput& input() {return m_input;}
    const UnityMath::Vector3D& position() const {return m_position;}
    const UnityMath::Vector3D& eulaAngle() const {return m_eulaAngle;}
    ClientInfo* client() const {return m_client;}

private:
    quint32 m_uuid;
    QString m_nickname;

    // 物理状态
    bool m_onGround = true;
    int m_jumpCount = 0;    // 连跳次数

    // 客户端输入
    PlayerInput m_input;

    // 期望移动
    UnityMath::Vector3D m_desiredVelocity;

    // 服务端计算状态
    UnityMath::Vector3D m_position;
    UnityMath::Vector3D m_eulaAngle;

    std::weak_ptr<CharacterProps> _characterProps;  // 当前选中角色属性，从角色管理器中获取角色属性
    ClientInfo* m_client = nullptr;
};
