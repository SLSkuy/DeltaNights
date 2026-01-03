/* ------------------------------------------------------------
 *  Author:  2023051604044 wanrui
 *  Date:  2025.12.28
 *  LastUpdate:  2026.1.2
 *
 *  Protbuf对象重置处理器
 * ------------------------------------------------------------ */

#pragma once

#include "../GameEvent/BattleSyncPackage.pb.h"
#include "protopool.h"

template<>
struct ProtoResetter<BattleSyncPackage::BattleSyncRequest>
{
    static void Reset(BattleSyncPackage::BattleSyncRequest* msg)
    {
        msg->Clear();
    }
};

template<>
struct ProtoResetter<BattleSyncPackage::BattleSyncResponse>
{
    static void Reset(BattleSyncPackage::BattleSyncResponse* msg)
    {
        msg->clear_states();
        msg->Clear();
    }
};
