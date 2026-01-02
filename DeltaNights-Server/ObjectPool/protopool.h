/* ------------------------------------------------------------
 *  Author:  2023051604044 wanrui
 *  Date:  2025.12.28
 *  LastUpdate:  2026.1.2
 *
 *  Protobuf对象池
 * ------------------------------------------------------------ */

#pragma once

#include "../GameEvent/SyncPackage.pb.h"
#include "../GameEvent/BattleSyncPackage.pb.h"
#include "objectpool.h"

class ProtoPool
{
public:
    static SyncPackage::LocalSyncPackage* AcquireLocal();
    static void Release(SyncPackage::LocalSyncPackage* pkg);

    static SyncPackage::RemoteSyncPackage* AcquireRemote();
    static void Release(SyncPackage::RemoteSyncPackage* pkg);

    static BattleSyncPackage::BattleSyncRequest* AcquireBattleReq();
    static void Release(BattleSyncPackage::BattleSyncRequest* req);

    static BattleSyncPackage::BattleSyncResponse* AcquireBattleResp();
    static void Release(BattleSyncPackage::BattleSyncResponse* resp);

private:
    static ObjectPool<SyncPackage::LocalSyncPackage> m_localPackages;
    static ObjectPool<SyncPackage::RemoteSyncPackage> m_remotePackages;
    static ObjectPool<BattleSyncPackage::BattleSyncRequest> m_battleReqs;
    static ObjectPool<BattleSyncPackage::BattleSyncResponse> m_battleResps;
};
