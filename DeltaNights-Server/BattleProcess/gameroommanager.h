/* ------------------------------------------------------------
 *  Author:  2023051604044 wanrui
 *  Date:  2025.12.23
 *  LastUpdate: 2025.12.23
 *
 *  游戏战局房间管理
 *  处理房间的创建、销毁、玩家与房间之间的交互逻辑
 *  获取每个房间的Tick事件，并传给网络分发层进行网络同步
 * ------------------------------------------------------------ */

#pragma once

#include <QObject>
#include <memory>
#include <unordered_map>

#include "gameroom.h"

class GameRoomManager : public QObject
{
    Q_OBJECT
public:
    GameRoomManager(QObject *parent = nullptr);

signals:
    void frameGenerated();  // 转发各个战局的信号

private:
    std::unordered_map<quint32, std::unique_ptr<GameRoom>> m_rooms; // roomID -> GameRoom
};
