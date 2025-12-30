/* ------------------------------------------------------------
 *  Author:  2023051604044 wanrui
 *  Date:  2025.12.23
 *  LastUpdate: 2025.12.23
 *
 *  碰撞检测系统
 *  用于处理所有的物理事件
 *  - 玩家移动
 *  - 地面检测
 *  - 重力
 *  - 碰撞检测
 * ------------------------------------------------------------ */

#pragma once

#include <unordered_map>
#include <QObject>

class GameMap;
class PlayerEntity;

struct RaycastResult
{
    // TODO: 射线检测结果
};

class CollisionSystem : public QObject
{
    Q_OBJECT
public:
    CollisionSystem();

    void update(const GameMap& map, const std::unordered_map<quint32, PlayerEntity*>& players);
    RaycastResult raycast();

private:
    void resolve(PlayerEntity& player, const GameMap& map, float deltaTime);
};
