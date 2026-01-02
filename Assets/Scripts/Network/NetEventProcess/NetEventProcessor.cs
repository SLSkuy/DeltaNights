/* ------------------------------------------------------------
 *  Author:  2023051604044 wanrui
 *  Date:  2025.12.31
 *  LastUpdate:  2025.12.31
 *
 *  网络双端通信示例
 * ------------------------------------------------------------ */

using ClientSyncPackage;
using UnityEngine;

namespace Network.NetEventProcess
{
    /// <summary>
    /// 网络通信传输示例
    /// </summary>
    public class NetEventProcessor : MonoBehaviour
    {
        void Start()
        {
            // 注册事件处理器
            NetWorkManager.instance.RegisterEventHandler<ConnectResponsePackage>(
                NetEvent.ConnectResponse
                , PrintConnectResponse);
        }

        // 事件处理函数
        void PrintConnectResponse(ConnectResponsePackage package)
        {
            Debug.Log($"[MessageProcessor] Connect request package {package.Content}");
        }
    }
}