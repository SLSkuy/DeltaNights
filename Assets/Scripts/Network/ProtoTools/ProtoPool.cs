/* ------------------------------------------------------------
 *  Author:  2023051604044 wanrui
 *  Date:  2025.12.31
 *  LastUpdate:  2025.12.31
 *
 *  Protobuf对象池
 *  负责管理创建与销毁Protobuf对象
 *  避免频繁的创建与销毁操作
 * ------------------------------------------------------------ */

using SyncPackage;
using ObjPool;

namespace Network.ProtoTools
{
    /// <summary>
    /// Protobuf对象池
    /// 缓存Protobuf对象复用
    /// 减少创建与销毁Protobuf对象
    /// </summary>
    public static class ProtoPool
    {
        /// <summary>
        /// 本地请求包对象池
        /// </summary>
        private static readonly ObjectPool<LocalSyncPackage> LocalSyncPackagePool =
            new(ProtoResetter.ResetLocalPackage, 64);

        /// <summary>
        /// 远程回应包对象池
        /// </summary>
        private static readonly ObjectPool<RemoteSyncPackage> RemoteSyncPackagePool =
            new(ProtoResetter.ResetRemotePackage, 64);

        /// <summary>
        /// 获取新的LocalSyncPackage包
        /// </summary>
        /// <returns></returns>
        public static LocalSyncPackage NewLocalSyncPackage()
        {
            // TODO: 快捷创建Protobuf对象
            return LocalSyncPackagePool.GetObject();
        }

        /// <summary>
        /// 拓展销毁方法，直接通过对象调用方法回收到对象池
        /// </summary>
        public static void Dispose(this LocalSyncPackage pkg)
        {
            if (pkg == null) return;
            
            LocalSyncPackagePool.ReturnObject(pkg);
        }

        /// <summary>
        /// 获取新的RemoteSyncPackage包
        /// </summary>
        /// <returns></returns>
        public static RemoteSyncPackage NewRemoteSyncPackage()
        {
            // TODO: 快捷创建Protobuf对象
            return RemoteSyncPackagePool.GetObject();
        }

        /// <summary>
        /// 拓展销毁方法，直接通过对象调用方法回收到对象池
        /// </summary>
        public static void Dispose(this RemoteSyncPackage pkg)
        {
            if (pkg == null) return;
            
            RemoteSyncPackagePool.ReturnObject(pkg);
        }
    }
}