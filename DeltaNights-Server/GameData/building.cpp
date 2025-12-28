#include "building.h"
#include "../CollisionSystem/collider.h"

Building::Building() {}

Building::~Building()
{
    delete _collider;
}
