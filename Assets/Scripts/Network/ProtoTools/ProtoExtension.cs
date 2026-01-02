/* ------------------------------------------------------------
 *  Author:  2023051604044 wanrui
 *  Date:  2025.12.31
 *  LastUpdate:  2025.12.31
 *
 *  拓展Protobuf方法
 * ------------------------------------------------------------ */

using Network.ProtoTools;
using ObjPool;

namespace SyncPackage
{
    public partial class LocalSyncPackage : IPoolable
    {
        public void Reset() => ProtoResetter.Reset(this);
    }
}

namespace BattleSyncPackage
{
    public partial class BattleSyncRequest : IPoolable
    {
        public void Reset() => ProtoResetter.Reset(this);
    }
}