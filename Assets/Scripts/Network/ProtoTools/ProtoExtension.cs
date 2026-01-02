/* ------------------------------------------------------------
 *  Author:  2023051604044 wanrui
 *  Date:  2025.12.31
 *  LastUpdate:  2026.1.2
 *
 *  拓展Protobuf方法
 * ------------------------------------------------------------ */

using Network.ProtoTools;
using ObjPool;

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