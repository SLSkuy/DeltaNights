/* ------------------------------------------------------------
 *  Author:  2023051604044 wanrui
 *  Date:  2025.12.31
 *  LastUpdate:  2025.12.31
 *
 *  Protobuf对象池
 *  负责管理创建与销毁Protobuf对象
 *  避免频繁的创建与销毁操作
 * ------------------------------------------------------------ */

using AckSyncPackage;
using BattleSyncPackage;
using LobbySyncPackage;
using SyncPackage;

namespace Network.ProtoTools
{
    /// <summary>
    /// Protobuf 对象池统一入口
    /// </summary>
    public static class ProtoPool
    {
        private static readonly ProtoObjectPool<LocalSyncPackage> LocalPool = new(24);
        private static readonly ProtoObjectPool<RemoteSyncPackage> RemotePool = new(24);

        private static readonly ProtoObjectPool<AckSyncRequest> AckReqPool = new(8);
        private static readonly ProtoObjectPool<BattleSyncRequest> BattleReqPool = new(8);
        private static readonly ProtoObjectPool<LobbySyncRequest> LobbyReqPool = new(8);

        private static readonly ProtoObjectPool<AckSyncResponse> AckRespPool = new(8);
        private static readonly ProtoObjectPool<BattleSyncResponse> BattleRespPool = new(8);
        private static readonly ProtoObjectPool<LobbySyncResponse> LobbyRespPool = new(8);

        #region 创建Protobuf包

        public static LocalSyncPackage NewLocal() => LocalPool.Get();
        public static RemoteSyncPackage NewRemote() => RemotePool.Get();

        public static AckSyncRequest NewAckReq() => AckReqPool.Get();
        public static BattleSyncRequest NewBattleReq() => BattleReqPool.Get();
        public static LobbySyncRequest NewLobbyReq() => LobbyReqPool.Get();

        public static AckSyncResponse NewAckResp() => AckRespPool.Get();
        public static BattleSyncResponse NewBattleResp() => BattleRespPool.Get();
        public static LobbySyncResponse NewLobbyResp() => LobbyRespPool.Get();

        #endregion

        #region 回收Protobuf包

        public static void Dispose(this LocalSyncPackage p) => LocalPool.Return(p);
        public static void Dispose(this RemoteSyncPackage p) => RemotePool.Return(p);

        public static void Dispose(this AckSyncRequest p) => AckReqPool.Return(p);
        public static void Dispose(this BattleSyncRequest p) => BattleReqPool.Return(p);
        public static void Dispose(this LobbySyncRequest p) => LobbyReqPool.Return(p);

        public static void Dispose(this AckSyncResponse p) => AckRespPool.Return(p);
        public static void Dispose(this BattleSyncResponse p) => BattleRespPool.Return(p);
        public static void Dispose(this LobbySyncResponse p) => LobbyRespPool.Return(p);

        #endregion
    }
}
