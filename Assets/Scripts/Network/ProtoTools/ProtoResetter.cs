using System;
using System.Collections.Generic;
using AckSyncPackage;
using BattleSyncPackage;
using LobbySyncPackage;
using SyncPackage;

namespace Network.ProtoTools
{
    /// <summary>
    /// Protobuf 重置逻辑集中管理
    /// </summary>
    internal static class ProtoResetter
    {
        #region LocalSyncPackage

        /// <summary>
        /// LocalSyncPackage重置处理事件存储
        /// </summary>
        private static readonly Dictionary<LocalSyncPackage.ContentOneofCase, Action<LocalSyncPackage>> LocalResetMap = new()
        {
            {
                LocalSyncPackage.ContentOneofCase.AckSync,
                p => { p.AckSync.Dispose(); p.AckSync = null; }
            },
            {
                LocalSyncPackage.ContentOneofCase.BattleSync,
                p => { p.BattleSync.Dispose(); p.BattleSync = null; }
            },
            {
                LocalSyncPackage.ContentOneofCase.LobbySync,
                p => { p.LobbySync.Dispose(); p.LobbySync = null; }
            }
        };

        /// <summary>
        /// LocalSyncPackage重置处理方法
        /// </summary>
        public static void Reset(LocalSyncPackage pkg)
        {
            if (pkg == null) return;

            if (LocalResetMap.TryGetValue(pkg.ContentCase, out var reset))
                reset(pkg);

            pkg.ClearContent();
            pkg.EventID = LocalSyncEvent.LocalNone;
        }

        #endregion

        #region AckSyncRequest

        /// <summary>
        /// AckSyncRequest重置处理事件存储
        /// </summary>
        private static readonly Dictionary<AckSyncRequest.ContentOneofCase, Action<AckSyncRequest>> AckReqResetMap = new()
        {
            { AckSyncRequest.ContentOneofCase.HeartBeat, p => p.HeartBeat = null },
            { AckSyncRequest.ContentOneofCase.Connect,   p => p.Connect = null }
        };

        /// <summary>
        /// AckSyncRequest重置处理方法
        /// </summary>
        public static void Reset(AckSyncRequest pkg)
        {
            if (pkg == null) return;

            if (AckReqResetMap.TryGetValue(pkg.ContentCase, out var reset))
                reset(pkg);

            pkg.ClearContent();
            pkg.EventID = LocalAckEvent.LocalAckNone;
        }

        #endregion

        #region BattleSyncRequest
        
        // /// <summary>
        // /// BattleSyncResponse重置处理事件存储
        // /// </summary>
        // private static readonly Dictionary<BattleSyncRequest.ContentOneofCase, Action<BattleSyncRequest>> BattleReqResetMap = new()
        // {
        //     
        // };

        /// <summary>
        /// BattleSyncRequest重置处理方法
        /// </summary>
        public static void Reset(BattleSyncRequest pkg)
        {
            if (pkg == null) return;
            
            // if (BattleReqResetMap.TryGetValue(pkg.ContentCase, out var reset))
            //     reset(pkg);
            
            // pkg.ClearContent();
            pkg.EventID = LocalBattleEvent.LocalBattleNone;
        }
        
        #endregion
        
        #region LobbySyncPackage
        
        // /// <summary>
        // /// LobbySyncRequest重置处理事件存储
        // /// </summary>
        // private static readonly Dictionary<LobbySyncRequest.ContentOneofCase, Action<LobbySyncRequest>> LobbyReqResetMap = new()
        // {
        //     
        // };

        /// <summary>
        /// LobbySyncRequest重置处理方法
        /// </summary>
        public static void Reset(LobbySyncRequest pkg)
        {
            if (pkg == null) return;
            
            // if (LobbyReqResetMap.TryGetValue(pkg.ContentCase, out var reset))
            //     reset(pkg);
            
            // pkg.ClearContent();
            pkg.EventID = LocalLobbyEvent.LocalLobbyNone;
        }

        #endregion

        #region RemoteSyncPackage

        /// <summary>
        /// RemoteSyncPackage重置处理事件存储
        /// </summary>
        private static readonly Dictionary<RemoteSyncPackage.ContentOneofCase, Action<RemoteSyncPackage>> RemoteResetMap = new()
        {
            {
                RemoteSyncPackage.ContentOneofCase.AckSync,
                p => { p.AckSync.Dispose(); p.AckSync = null; }
            },
            {
                RemoteSyncPackage.ContentOneofCase.BattlePackage,
                p => { p.BattlePackage.Dispose(); p.BattlePackage = null; }
            },
            {
                RemoteSyncPackage.ContentOneofCase.LobbyPackage,
                p => { p.LobbyPackage.Dispose(); p.LobbyPackage = null; }
            }
        };

        /// <summary>
        /// RemoteSyncPackage重置处理方法
        /// </summary>
        public static void Reset(RemoteSyncPackage pkg)
        {
            if (pkg == null) return;

            if (RemoteResetMap.TryGetValue(pkg.ContentCase, out var reset))
                reset(pkg);

            pkg.ClearContent();
            pkg.EventID = RemoteSyncEvent.RemoteNone;
        }

        #endregion

        #region AckSyncResponse
        
        // /// <summary>
        // /// AckSyncResponse重置处理事件存储
        // /// </summary>
        // private static readonly Dictionary<AckSyncResponse.ContentOneofCase, Action<AckSyncResponse>> AckResponseResetMap = new()
        // {
        //     
        // };

        /// <summary>
        /// AckSyncResponse重置处理方法
        /// </summary>
        public static void Reset(AckSyncResponse pkg)
        {
            if (pkg == null) return;

            // if (AckResponseResetMap.TryGetValue(pkg.ContentCase, out var reset))
            //     reset(pkg);
            
            pkg.ClearContent();
            pkg.EventID = RemoteAckEvent.RemoteAckNone;
        }
        
        #endregion
        
        #region BattleSyncResponse
        
        // /// <summary>
        // /// AckSyncResponse重置处理事件存储
        // /// </summary>
        // private static readonly Dictionary<BattleSyncResponse.ContentOneofCase, Action<BattleSyncResponse>> BattleResponseResetMap = new()
        // {
        //     
        // };
        
        /// <summary>
        /// BattleSyncResponse重置处理事件存储
        /// </summary>
        public static void Reset(BattleSyncResponse pkg)
        {
            if (pkg == null) return;
            
            // if (BattleResponseResetMap.TryGetValue(pkg.ContentCase, out var reset))
            //     reset(pkg);
            
            // pkg.ClearContent();
            pkg.EventID = RemoteBattleEvent.RemoteBattleNone;
        }

        #endregion
        
        #region LobbySyncResponse
        
        // /// <summary>
        // /// AckSyncResponse重置处理事件存储
        // /// </summary>
        // private static readonly Dictionary<LobbySyncResponse.ContentOneofCase, Action<LobbySyncResponse>> LobbyResponseResetMap = new()
        // {
        //     
        // };
        
        public static void Reset(LobbySyncResponse pkg)
        {
            if (pkg == null) return;
            
            // if (LobbyResponseResetMap.TryGetValue(pkg.ContentCase, out var reset))
            //     reset(pkg);
            
            // pkg.ClearContent();
            pkg.EventID = RemoteLobbyEvent.RemoteLobbyNone;
        }

        #endregion
    }
}
