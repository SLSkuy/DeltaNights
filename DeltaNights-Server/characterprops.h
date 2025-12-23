/* ------------------------------------------------------------
 *  Author:  2023051604044 wanrui
 *  Date:  2025.12.23
 *  LastUpdate: 2025.12.23
 *
 *  玩家战局内角色属性
 *  用于计算战局内状态
 * ------------------------------------------------------------ */

#pragma once

#include <QObject>
#include <QString>

class CharacterProps : public QObject
{
    Q_OBJECT
public:
    CharacterProps();
private:
    QString name;   // 角色名称

    // 移动属性
    float speed = 5.0;
    float shoulderAimSpeed = 3.0;
    float aimSpeed = 2.2;
    float jumpSpeed = 6.0;
    float locomotionDamping = 0.2;

    // 空中位移属性
    int maxJumpCount = 2;
    float airMoveFactor = 1.0;

    // 重力属性
    float gravity = 12.0;
};
