/* ------------------------------------------------------------
 *  Author:  2023051604044 wanrui
 *  Date:  2025.12.23
 *  LastUpdate: 2025.12.28
 *
 *  战局内玩家抽象实体
 *  记录客户端输入
 *  计算玩家状态并同步给客户端
 * ------------------------------------------------------------ */

#pragma once

#include <QObject>
#include <memory.h>

#include "../BattleSyncPackage.pb.h"
#include "../GameData/characterprops.h"

class ClientInfo;

class PlayerEntity : public QObject
{
    Q_OBJECT
public:
    explicit PlayerEntity(QObject *parent = nullptr);

    void tick();        // 逻辑更新，生成期望位移等交给碰撞系统处理

    void bindClient(ClientInfo* client);

public:
    quint32 uuid() const {return m_uuid;}
    bool jump() const {return m_jump;}
    float yaw() const {return m_yaw;}
    float pitch() const {return m_pitch;}
    const BattleSyncPackage::Vector3D& position() const {return m_position;}
    const BattleSyncPackage::Vector3D& eulaAngle() const {return m_eulaAngle;}
    ClientInfo* client() const {return m_client;}

private:
    quint32 m_uuid;

    // 物理状态
    bool m_onGround = true;
    int m_jumpCount = 0;    // 连跳次数

    // 客户端输入
    BattleSyncPackage::Vector2D m_moveDir;
    bool m_jump;
    float m_yaw;
    float m_pitch;

    // 期望移动
    BattleSyncPackage::Vector3D m_desiredVelocity;

    // 服务端计算状态
    BattleSyncPackage::Vector3D m_position;
    BattleSyncPackage::Vector3D m_eulaAngle;

    std::weak_ptr<CharacterProps> _characterProps;  // 当前选中角色属性，从角色管理器中获取角色属性
    ClientInfo* m_client = nullptr;
};
