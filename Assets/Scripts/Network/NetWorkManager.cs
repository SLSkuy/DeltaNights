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

        [Header("玩家信息")] 
        public uint clientID = 0;

        [Header("网络属性配置")] 
        [SerializeField] [Tooltip("网路心跳间隔")] private float heartBeatStep = 1f;

        // 组件引用
        private TcpManager _tcp;
        private UdpManager _udp;
        private MessageProcessor _processor;

        // Unity主线程处理调用队列
        private readonly ConcurrentQueue<byte[]> _mainThreadQueue = new();

        // 心跳包缓存
        private LocalSyncPackage _heartBeatPackage;
        private float _heartBeatTimer;
        
        #region 成员方法

        /// <summary>
        /// UDP发送Protobuf事件
        /// </summary>
        public void SendUdp(LocalSyncPackage syncPackage)
        {
            _udp.EnqueueSendProtobuf(syncPackage);
        }

        /// <summary>
        /// TCP发送Protobuf事件
        /// </summary>
        public void SendTcp(LocalSyncPackage syncPackage)
        {
            _tcp.EnqueueSendProtobuf(syncPackage);
        }

        /// <summary>
        /// 心跳包，每隔一段固定时间进行发送
        /// </summary>
        private void HeartBeat()
        {
            // 断开连接时暂停发送心跳包
            if (!_tcp.Connected) return;
            
            _heartBeatPackage ??= new LocalSyncPackage
            {
                EventID = LocalSyncEvent.AckRequest,
                AckSync = new AckSyncRequest
                {
                    EventID = LocalAckEvent.HeartBeat,
                    HeartBeat = new HeartBeatPackage
                    {
                        ClientID = clientID
                    }
                }
            };
            
            // 发送心跳包
            SendTcp(_heartBeatPackage);
            _heartBeatTimer = heartBeatStep;
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
                EventID = LocalSyncEvent.AckRequest,
                AckSync = new AckSyncRequest
                {
                    EventID = LocalAckEvent.Connect,
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
            // 消息队列处理
            while (_mainThreadQueue.Count > 0)
            {
                if (_mainThreadQueue.TryDequeue(out var data))
                {
                    _processor.DeSerialize(data);
                }
            }

            // 心跳包处理
            if (_heartBeatTimer > 0)
                _heartBeatTimer -= Time.deltaTime;
            else
                HeartBeat();
        }

        void OnDestroy()
        {
            _tcp?.Disconnect();
            _udp?.Stop();
        }
        
        #endregion
    }
}