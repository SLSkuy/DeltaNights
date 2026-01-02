#include "protopool.h"

ObjectPool<BattleSyncPackage::BattleSyncRequest> ProtoPool::m_battleReqs(64);
ObjectPool<BattleSyncPackage::BattleSyncResponse> ProtoPool::m_battleResps(64);

BattleSyncPackage::BattleSyncRequest* ProtoPool::AcquireBattleReq() { return m_battleReqs.Acquire(); }
void ProtoPool::Release(BattleSyncPackage::BattleSyncRequest* req) { m_battleReqs.Release(req); }

BattleSyncPackage::BattleSyncResponse* ProtoPool::AcquireBattleResp() { return m_battleResps.Acquire(); }
void ProtoPool::Release(BattleSyncPackage::BattleSyncResponse* resp) { m_battleResps.Release(resp); }
