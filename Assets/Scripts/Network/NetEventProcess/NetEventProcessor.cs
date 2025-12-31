using AckSyncPackage;
using UnityEngine;

namespace Network.NetEventProcess
{
    public class NetEventProcessor : MonoBehaviour
    {
        void Start()
        {
            NetWorkManager.instance.RegisterEventHandler<ConnectResponsePackage>(
                NetEvent.ConnectResponse
                , PrintConnectResponse);
        }

        void PrintConnectResponse(ConnectResponsePackage package)
        {
            Debug.Log($"[MessageProcessor] Connect request package {package.Content}");
        }
    }
}