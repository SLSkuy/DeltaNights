/* ------------------------------------------------------------
 *  Author:  2023051604044 wanrui
 *  Date:  2025.12.31
 *  LastUpdate:  2025.12.31
 *
 *  负责Protobuf对象的重置处理
 *  以回收进对象池实现复用
 * ------------------------------------------------------------ */

using AckSyncPackage;
using BattleSyncPackage;
using LobbySyncPackage;
using SyncPackage;

namespace Network.ProtoTools
{
    /// <summary>
    /// Protobuf对象重置工具
    /// 负责快捷重置Protobuf对象
    /// </summary>
    public static class ProtoResetter
    {
        /* ==================================================
          本地同步包重置处理
        ================================================== */
        public static void ResetLocalPackage(LocalSyncPackage pkg)
        {
            switch (pkg.EventID)
            {
                case LocalSyncEvent.LocalNone:
                    break;
                case LocalSyncEvent.AckRequest:
                    ResetAckRequestPackage(pkg.AckSync);
                    break;
                case LocalSyncEvent.BattleRequest:
                    ResetBattleRequestPackage(pkg.BattleSync);
                    break;
                case LocalSyncEvent.LobbyRequest:
                    ResetLobbyRequestPackage(pkg.LobbySync);
                    break;
                default:
                    break;
            }
            pkg.EventID = LocalSyncEvent.LocalNone;
        }

        public static void ResetAckRequestPackage(AckSyncRequest pkg)
        {
            switch (pkg.EventID)
            {
                case LocalAckEvent.LocalAckNone:
                    break;
                case LocalAckEvent.HeartBeat:
                    pkg.HeartBeat.ClientID = 0;
                    break;
                case LocalAckEvent.ConnectRequest:
                    pkg.Connect.Port = 0;
                    break;
                default:
                    break;
            }
            pkg.EventID = LocalAckEvent.LocalAckNone;
        }

        public static void ResetBattleRequestPackage(BattleSyncRequest pkg)
        {
            switch (pkg.EventID)
            {
                case LocalBattleEvent.LocalBattleNone:
                    break;
                default:
                    break;
            }
            pkg.EventID = LocalBattleEvent.LocalBattleNone;
        }

        public static void ResetLobbyRequestPackage(LobbySyncRequest pkg)
        {
            switch (pkg.EventID)
            {
                case LocalLobbyEvent.LocalLobbyNone:
                    break;
                default:
                    break;
            }
            pkg.EventID = LocalLobbyEvent.LocalLobbyNone;
        }
        
        /* ==================================================
          远程同步包重置处理
        ================================================== */
        public static void ResetRemotePackage(RemoteSyncPackage pkg)
        {
            
        }

        public static void ResetAckResponsePackage(AckSyncResponse pkg)
        {
            
        }

        public static void ResetBattleResponsePackage(BattleSyncResponse pkg)
        {
            
        }

        public static void ResetLobbyResponsePackage(LobbySyncResponse pkg)
        {
            
        }
    }
}