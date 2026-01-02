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
        /// BattleSyncRequest重置处理方法
        /// </summary>
        public static void Reset(BattleSyncRequest pkg)
        {
            if (pkg == null) return;
            // TODO: 重置BattleSyncRequest包中内容
        }
        
        /// <summary>
        /// BattleSyncResponse重置处理方法
        /// </summary>
        public static void Reset(BattleSyncResponse pkg)
        {
            if (pkg == null) return;
            // TODO: 重置BattleSyncRequest包中内容
        }
    }
}
