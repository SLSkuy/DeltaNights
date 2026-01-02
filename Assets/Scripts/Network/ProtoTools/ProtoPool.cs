/* ------------------------------------------------------------
 *  Author:  2023051604044 wanrui
 *  Date:  2025.12.31
 *  LastUpdate:  2025.12.31
 *
 *  Protobuf对象池
 *  负责管理创建与销毁Protobuf对象
 *  避免频繁的创建与销毁操作
 * ------------------------------------------------------------ */

using BattleSyncPackage;
using SyncPackage;

namespace Network.ProtoTools
{
    /// <summary>
    /// Protobuf 对象池统一入口
    /// </summary>
    public static class ProtoPool
    {
        // 本地UDP战局高频同步使用对象池，其余没必要使用
        private static readonly ProtoObjectPool<LocalSyncPackage> LocalPool = new(64);
        private static readonly ProtoObjectPool<BattleSyncRequest> BattleReqPool = new(32);

        #region 创建Protobuf包

        public static LocalSyncPackage NewLocal() => LocalPool.Get();
        public static BattleSyncRequest NewBattleReq() => BattleReqPool.Get();
        
        #endregion

        #region 回收Protobuf包

        public static void Dispose(this LocalSyncPackage p) => LocalPool.Return(p);
        public static void Dispose(this BattleSyncRequest p) => BattleReqPool.Return(p);

        #endregion
    }
}
