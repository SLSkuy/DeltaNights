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