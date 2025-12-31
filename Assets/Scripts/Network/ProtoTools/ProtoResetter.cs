using System;
using System.Collections.Generic;
using BattleSyncPackage;
using SyncPackage;

namespace Network.ProtoTools
{
    /// <summary>
    /// Protobuf 重置逻辑集中管理
    /// </summary>
    internal static class ProtoResetter
    {
        /// <summary>
        /// LocalSyncPackage重置处理事件存储
        /// </summary>
        private static readonly Dictionary<LocalSyncPackage.ContentOneofCase, Action<LocalSyncPackage>> LocalResetMap = new()
        {
            {
                LocalSyncPackage.ContentOneofCase.AckSync,
                p => { p.AckSync = null; }
            },
            {
                // 战局包回收对象池处理
                LocalSyncPackage.ContentOneofCase.BattleSync,
                p => { p.BattleSync.Dispose(); p.BattleSync = null; }
            },
            {
                LocalSyncPackage.ContentOneofCase.LobbySync,
                p => { p.LobbySync = null; }
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

        /// <summary>
        /// BattleSyncRequest重置处理方法
        /// </summary>
        public static void Reset(BattleSyncRequest pkg)
        {
            if (pkg == null) return;
            // TODO: 重置BattleSyncRequest包中内容
        }
    }
}
