/* ------------------------------------------------------------
 *  Author:  2023051604044 wanrui
 *  Date:  2025.10.28
 *  LastUpdate:  2025.12.22
 * 
 *  功能简述：
 *  NetWorkManager 负责管理客户端的网络连接与消息通信，
 *  提供与服务器之间的基础数据收发能力。
 *
 *  主要功能：
 *  - 建立并维护 TCP 网络连接
 *  - 异步接收服务器消息并转发至主线程
 *  - 对外提供消息发送与接收事件接口
 *
 *  使用说明：
 *  - 场景中仅允许存在一个 NetWorkManager 实例
 *  - 网络通信通过事件回调的方式分发给其他系统
 *  - 不直接在网络线程中处理游戏逻辑
 * ------------------------------------------------------------ */

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Network
{
    public class NetWorkManager : MonoBehaviour
    {
        public static NetWorkManager Instance;
        
        [Header("服务器配置")]
        public string serverIp = "127.0.0.1";
        public short serverPort = 11451;

        public event Action<string> ReceiveMessage;
        
        private Socket _socket;
        private CancellationTokenSource _cts;

        private Queue<string> _receiveMsgs;
        
        void Awake()
        {
            _receiveMsgs = new Queue<string>();
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Instance = this;
            
            DontDestroyOnLoad(gameObject);
            
            ConnectToServer(serverIp, serverPort);
        }

        void Start()
        {
            _cts = new CancellationTokenSource();
            _ = Task.Run(()=>ReceiveMsg(_cts.Token));
        }

        void Update()
        {
            while (_receiveMsgs.Count > 0)
            {
                string msg = _receiveMsgs.Dequeue();
                ReceiveMessage?.Invoke(msg); // 在主线程触发
                Debug.Log($"Received From Server: {msg}");
            }
        }

        private void ConnectToServer(string ip, short port)
        {
            if (_socket.Connected)
            {
                _socket.Shutdown(SocketShutdown.Both);
                _socket.Close();
            }

            try
            {
                _socket.Connect(IPAddress.Parse(ip), port);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        public void SendMsg(string msg)
        {
            if (_socket is { Connected: true })
            {
                _socket.Send(Encoding.UTF8.GetBytes(msg));
            }
        }

        private async Task ReceiveMsg(CancellationToken token)
        {
            byte[] msgBytes = new byte[1024 * 1024];
            while (!token.IsCancellationRequested)
            {
                try
                {
                    var len = await _socket.ReceiveAsync(msgBytes, SocketFlags.None, token);
                    if (len <= 0)
                    {
                        Debug.LogWarning("Server Closed");
                        break;
                    }

                    var msg = Encoding.UTF8.GetString(msgBytes, 0, len);
                    _receiveMsgs.Enqueue(msg);
                    Debug.Log($"Received From Server: {msg}");
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                    break;
                }
            }
        }

        private void OnDestroy()
        {
            _cts?.Cancel();
            
            try
            {
                if (_socket == null || !_socket.Connected) return;
                
                _socket.Shutdown(SocketShutdown.Both);
                _socket.Close();
            }catch(Exception e)
            {
                Debug.LogError(e);
            }
        }
    }
}
