/* ------------------------------------------------------------
 *  Author:  2023051604044 wanrui
 *  Date:  2025.12.28
 *  LastUpdate:  2026.1.2
 *
 *  Protbuf对象重置处理器
 * ------------------------------------------------------------ */

#pragma once

#include "../GameEvent/BattleSyncPackage.pb.h"
#include "../GameEvent/SyncPackage.pb.h"
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
        msg->Clear();
    }
};

template<>
struct ProtoResetter<SyncPackage::LocalSyncPackage>
{
    static void Reset(SyncPackage::LocalSyncPackage* pkg)
    {
        switch (pkg->content_case())
        {
            case SyncPackage::LocalSyncPackage::kBattleSync:
                ProtoPool::Release(pkg->release_battlesync());
                break;

            case SyncPackage::LocalSyncPackage::kLobbySync:
                pkg->clear_lobbysync();
                break;

            case SyncPackage::LocalSyncPackage::kAckSync:
                pkg->clear_acksync();
                break;

            default:
                break;
        }

        pkg->Clear();
        pkg->set_eventid(SyncPackage::Local_None);
    }
};

template<>
struct ProtoResetter<SyncPackage::RemoteSyncPackage>
{
    static void Reset(SyncPackage::RemoteSyncPackage* pkg)
    {
        switch (pkg->content_case())
        {
            case SyncPackage::RemoteSyncPackage::kBattlePackage:
                ProtoPool::Release(pkg->release_battlepackage());
                break;

            case SyncPackage::RemoteSyncPackage::kLobbyPackage:
                pkg->clear_lobbypackage();
                break;

            case SyncPackage::RemoteSyncPackage::kAckSync:
                pkg->clear_acksync();
                break;

            default:
                break;
        }

        pkg->Clear();
        pkg->set_eventid(SyncPackage::Remote_None);
    }
};
