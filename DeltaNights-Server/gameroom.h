/* ------------------------------------------------------------
 *  Author:  2023051604044 wanrui
 *  Date:  2025.12.23
 *  LastUpdate: 2025.12.23
 *
 *  游戏战局房间示例
 *  处理每一个战局的逻辑事件
 *  计算玩家的状态
 * ------------------------------------------------------------ */

#pragma once

#include <QObject>
#include <unordered_map>

class ClientInfo;
class Player;

class GameRoom : public QObject
{
    Q_OBJECT
public:
    GameRoom(QObject *parent = nullptr);
private:
    int m_roomCode;

    int m_playerCount = 0;

    std::unordered_map<QString, Player*> m_players; // IPAddress -> Player
};
