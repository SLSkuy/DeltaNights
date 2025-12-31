using Network.ProtoTools;
using ObjPool;

namespace SyncPackage
{
    public partial class LocalSyncPackage : IPoolable
    {
        public void Reset() => ProtoResetter.Reset(this);
    }

    public partial class RemoteSyncPackage : IPoolable
    {
        public void Reset() => ProtoResetter.Reset(this);
    }
}

namespace AckSyncPackage
{
    public partial class AckSyncRequest : IPoolable
    {
        public void Reset() => ProtoResetter.Reset(this);
    }

    public partial class AckSyncResponse : IPoolable
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

    public partial class BattleSyncResponse : IPoolable
    {
        public void Reset() => ProtoResetter.Reset(this);
    }
}

namespace LobbySyncPackage
{
    public partial class LobbySyncRequest : IPoolable
    {
        public void Reset() => ProtoResetter.Reset(this);
    }

    public partial class LobbySyncResponse : IPoolable
    {
        public void Reset() => ProtoResetter.Reset(this);
    }
}