/* ------------------------------------------------------------
 *  Author:  2023051604044 wanrui
 *  Date:  2025.10.28
 *  LastUpdate:  2025.12.30
 *
 *  功能简述：
 *  NetWorkManager 负责管理客户端的网络连接与消息通信，
 *  提供与服务器之间的基础数据收发能力。
 *
 *  主要功能：
 *  - 建立并维护 UDP、TCP 网络连接
 *  - 异步接收服务器消息并转发至主线程
 *  - 对外提供消息发送与接收事件接口
 *
 *  使用说明：
 *  - 场景中仅允许存在一个 NetWorkManager 实例
 *  - 网络通信通过事件回调的方式分发给其他系统
 *  - 不直接在网络线程中处理游戏逻辑
 * ------------------------------------------------------------ */

using System.Collections.Concurrent;
using System.Text;
using AckPackage;
using SyncPackage;
using UnityEngine;

namespace Network
{
    public class NetWorkManager : MonoBehaviour
    {
        public static NetWorkManager Instance;
        
        [Header("服务器配置")]
        [SerializeField]private string ip = "127.0.0.1";
        [SerializeField]private short tcpPort = 11451;
        [SerializeField]private short udpPort = 19198;

        [Header("网络属性配置")] 
        [SerializeField] [Tooltip("网路心跳间隔")] private float heartBeatStep = 1f;

        private TcpManager _tcp;
        private UdpManager _udp;
        private MessageProcessor _processor;

        private readonly ConcurrentQueue<byte[]> _mainThreadQueue = new();    // Unity主线程处理调用事件

        #region 成员方法

        public void SendUdp(LocalSyncPackage syncPackage)
        {
            _udp.EnqueueSendProtobuf(syncPackage);
        }

        public void SendTcp(LocalSyncPackage syncPackage)
        {
            _tcp.EnqueueSendProtobuf(syncPackage);
        }

        #endregion
        
        #region 周期函数
        
        void Awake()
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            _processor = new MessageProcessor();

            _tcp = new TcpManager();
            _tcp.OnMessageReceived += data => _mainThreadQueue.Enqueue(data);

            _udp = new UdpManager();
            _udp.OnDataReceived += data => _mainThreadQueue.Enqueue(data);

            _udp.Start(ip, udpPort);    // 启动UDP连接
            _tcp.Connect(ip, tcpPort);   // 开启TCP监听
        }

        void Start()
        {
            // 测试使用
            _udp.EnqueueSend(Encoding.UTF8.GetBytes("UDP连接测试"));

            LocalSyncPackage syncPackage = new LocalSyncPackage
            {
                EventID = LocalSyncEvent.Ack,
                AckSync = new AckSyncRequest
                {
                    EventID = AckSyncEvent.Connect,
                    Connect = new ConnectPackage
                    {
                        Port = _udp.UdpPort
                    }
                }
            };
            _tcp.EnqueueSendProtobuf(syncPackage);
        }

        void Update()
        {
            while (_mainThreadQueue.Count > 0)
            {
                if (_mainThreadQueue.TryDequeue(out var data))
                {
                    _processor.DeSerialize(data);
                }
            }
        }

        void OnDestroy()
        {
            _tcp?.Disconnect();
            _udp?.Stop();
        }
        
        #endregion
    }
}