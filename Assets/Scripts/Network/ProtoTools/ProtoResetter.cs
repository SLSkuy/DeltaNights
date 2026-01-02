/* ------------------------------------------------------------
 *  Author:  2023051604044 wanrui
 *  Date:  2025.12.28
 *  LastUpdate:  2026.1.2
 *
 *  Protobuf对象重置处理
 * ------------------------------------------------------------ */

using BattleSyncPackage;
using SyncPackage;
using UnityEngine;

namespace Network.ProtoTools
{
    /// <summary>
    /// Protobuf 重置逻辑集中管理
    /// </summary>
    internal static class ProtoResetter
    {
        /// <summary>
        /// LocalSyncPackage重置处理方法
        /// </summary>
        public static void Reset(LocalSyncPackage pkg)
        {
            if (pkg == null) return;

            switch (pkg.ContentCase)
            {
                case LocalSyncPackage.ContentOneofCase.AckSync:
                    pkg.AckSync = null;
                    break;
                case LocalSyncPackage.ContentOneofCase.BattleSync:
                    pkg.Dispose();
                    pkg.BattleSync = null;
                    break;
                case LocalSyncPackage.ContentOneofCase.LobbySync:
                    pkg.LobbySync = null;
                    break;
                default:
                    Debug.LogError("[ProtoResetter] Unknown content case: " + pkg.ContentCase);
                    break;
            }

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
