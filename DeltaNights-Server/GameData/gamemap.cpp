#include "gamemap.h"
#include "../GameData/building.h"

GameMap::GameMap(QObject* parent)
    : QObject(parent)
{
}

GameMap::~GameMap()
{
    for(auto& it:m_buildings)
    {
        delete it;
    }
}
