/* ------------------------------------------------------------
 *  Author:  2023051604044 wanrui
 *  Date:  2025.12.23
 *  LastUpdate: 2025.12.23
 *
 *  游戏战局房间管理
 *  处理房间的创建、销毁、玩家与房间之间的交互逻辑
 * ------------------------------------------------------------ */

#pragma once

#include <QObject>
#include <QHostAddress>
#include <QUdpSocket>
#include <memory>
#include <unordered_map>

#include "gameroom.h"

class GameRoomManager : public QObject
{
    Q_OBJECT
public:
    GameRoomManager(QObject *parent = nullptr);
private:
    std::unordered_map<int, std::unique_ptr<GameRoom>> m_rooms;
};
