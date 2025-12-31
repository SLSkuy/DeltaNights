/* ------------------------------------------------------------
 *  Author:  2023051604044 wanrui
 *  Date:  2025.12.28
 *  LastUpdate: 2025.12.28
 *
 *  战局地图抽象
 *  记录地图中所有的建筑对象
 *  便于进行碰撞检测
 * ------------------------------------------------------------ */

#pragma once

#include <vector>
#include <QObject>

class Building;

class GameMap : public QObject
{
    Q_OBJECT
public:
    explicit GameMap(QObject* parent = nullptr);
    ~GameMap();

private:
    int m_mapCode;
    QString m_mapName;

    std::vector<Building*> m_buildings;
};
