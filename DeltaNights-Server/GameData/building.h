/* ------------------------------------------------------------
 *  Author:  2023051604044 wanrui
 *  Date:  2025.12.28
 *  LastUpdate: 2025.12.28
 *
 *  单一建筑抽象
 *  记录每一个建筑碰撞体数据
 *  在开火事件触发时进行射线检测
 * ------------------------------------------------------------ */

#pragma once

class Collider;

class Building
{
public:
    Building();
private:
    Collider* _collider = nullptr;
};
