#include "protopool.h"

ObjectPool<SyncPackage::LocalSyncPackage> ProtoPool::m_localPackages(64);
ObjectPool<SyncPackage::RemoteSyncPackage> ProtoPool::m_remotePackages(64);
ObjectPool<BattleSyncPackage::BattleSyncRequest> ProtoPool::m_battleReqs(32);
ObjectPool<BattleSyncPackage::BattleSyncResponse> ProtoPool::m_battleResps(32);

SyncPackage::LocalSyncPackage* ProtoPool::AcquireLocal()  { return m_localPackages.Acquire(); }
void ProtoPool::Release(SyncPackage::LocalSyncPackage* pkg) { m_localPackages.Release(pkg); }

SyncPackage::RemoteSyncPackage* ProtoPool::AcquireRemote() { return m_remotePackages.Acquire(); }
void ProtoPool::Release(SyncPackage::RemoteSyncPackage* pkg) { m_remotePackages.Release(pkg); }

BattleSyncPackage::BattleSyncRequest* ProtoPool::AcquireBattleReq() { return m_battleReqs.Acquire(); }
void ProtoPool::Release(BattleSyncPackage::BattleSyncRequest* req) { m_battleReqs.Release(req); }

BattleSyncPackage::BattleSyncResponse* ProtoPool::AcquireBattleResp() { return m_battleResps.Acquire(); }
void ProtoPool::Release(BattleSyncPackage::BattleSyncResponse* resp) { m_battleResps.Release(resp); }
