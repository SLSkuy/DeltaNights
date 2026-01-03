/* ------------------------------------------------------------
 *  Author:  2023051604044 wanrui
 *  Date:  2025.12.31
 *  LastUpdate:  2026.1.2
 *
 *  Protobuf对象池
 *  负责管理创建与销毁Protobuf对象
 *  避免频繁的创建与销毁操作
 * ------------------------------------------------------------ */

using BattleSyncPackage;

namespace Network.ProtoTools
{
    /// <summary>
    /// Protobuf 对象池统一入口
    /// </summary>
    public static class ProtoPool
    {
        // 本地UDP战局高频同步使用对象池，其余没必要使用
        private static readonly ProtoObjectPool<BattleSyncRequest> BattleReqPool = new(32);
        private static readonly ProtoObjectPool<BattleSyncResponse> BattleRespPool = new(32);

        #region 创建Protobuf包
        
        public static BattleSyncRequest NewBattleReq() => BattleReqPool.Get();
        public static BattleSyncResponse NewBattleResp() => BattleRespPool.Get();
        
        #endregion

        #region 回收Protobuf包
        
        public static void Dispose(this BattleSyncRequest p) => BattleReqPool.Return(p);
        public static void Dispose(this BattleSyncResponse p) => BattleRespPool.Return(p);

        #endregion
    }
}
