/* ------------------------------------------------------------
 *  Author:  2023051604044 wanrui
 *  Date:  2025.12.28
 *  LastUpdate:  2026.1.2
 *
 *  Protobuf对象池
 *  只池化管理UDP战局同步
 * ------------------------------------------------------------ */

#pragma once

#include "../GameEvent/BattleSyncPackage.pb.h"
#include "objectpool.h"

class ProtoPool
{
public:
    static BattleSyncPackage::BattleSyncRequest* AcquireBattleReq();
    static void Release(BattleSyncPackage::BattleSyncRequest* req);

    static BattleSyncPackage::BattleSyncResponse* AcquireBattleResp();
    static void Release(BattleSyncPackage::BattleSyncResponse* resp);

private:
    static ObjectPool<BattleSyncPackage::BattleSyncRequest> m_battleReqs;
    static ObjectPool<BattleSyncPackage::BattleSyncResponse> m_battleResps;
};
