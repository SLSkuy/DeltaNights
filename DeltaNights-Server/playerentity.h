/* ------------------------------------------------------------
 *  Author:  2023051604044 wanrui
 *  Date:  2025.12.23
 *  LastUpdate: 2025.12.23
 *
 *  战局内玩家抽象实体
 *  记录客户端输入
 *  计算玩家状态并同步给客户端
 * ------------------------------------------------------------ */

#pragma once

#include <QObject>
#include <memory.h>

#include "BattleSyncPackage.pb.h"
#include "characterprops.h"

class PlayerEntity : public QObject
{
    Q_OBJECT
public:
    explicit PlayerEntity(QObject *parent = nullptr);
private:
    quint32 uuid;

    // 客户端输入
    BattleSyncPackage::Vector2D moveDir;
    bool jump;
    float yaw;
    float pitch;

    // 服务端计算状态
    BattleSyncPackage::Vector3D position;
    BattleSyncPackage::Vector3D eulaAngle;

    std::weak_ptr<CharacterProps> _characterProps;  // 当前选中角色属性，从角色管理器中获取角色属性
};
