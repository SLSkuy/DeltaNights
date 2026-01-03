#include "playerinfo.h"

PlayerInfo::PlayerInfo(quint32 uuid, QObject* parent)
    : m_uuid(uuid), QObject(parent)
{
}
