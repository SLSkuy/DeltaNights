/* ------------------------------------------------------------
 *  Author:  2023051604044 wanrui
 *  Date:  2025.12.28
 *  LastUpdate:  2026.1.2
 *
 *  封装UDP
 *  负责处理所有的UDP网络传输事件
 *  实现可靠的UDP以保证服务器逻辑同步的实时性与可靠性
 * ------------------------------------------------------------ */

using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using BattleSyncPackage;
using Google.Protobuf;

namespace Network
{
    public class UdpManager
    {
        private Socket _socket;
        private EndPoint _serverEndPoint;
        private CancellationTokenSource _cts;

        private const int BUFFER_SIZE = 1024 * 64;
        
        // 发送队列
        private readonly ConcurrentQueue<byte[]> _sendQueue = new();
        private readonly SemaphoreSlim _sendSignal = new(0);
        private Task _sendTask;

        #region 事件
        
        public event Action<byte[]> OnDataReceived;

        #endregion

        #region UDP控制
        
        /// <summary>
        /// 获取UDP绑定的端口
        /// </summary>
        public int UdpPort
        {
            get
            {
                if (_socket?.LocalEndPoint is IPEndPoint ep) return ep.Port;
                return -1;
            }
        }

        /// <summary>
        /// 开启UDP连接
        /// </summary>
        /// <param name="serverIp">服务器IP地址</param>
        /// <param name="serverPort">服务器端口</param>
        public void Start(string serverIp, int serverPort)
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            _socket.Bind(new IPEndPoint(IPAddress.Any, 0)); // 本地绑定udp链接
            
            // 设定服务器端点
            _serverEndPoint = new IPEndPoint(IPAddress.Parse(serverIp), serverPort);

            _cts = new CancellationTokenSource();
            _ = Task.Run(() => ReceiveLoop(_cts.Token));
            _ =  Task.Run(() => SendLoop(_cts.Token));
        }
        
        public void Stop()
        {
            _cts?.Cancel();
            _sendSignal?.Release();
            _socket?.Close();
            _socket = null;
        }
        
        /// <summary>
        /// 异步发送UDP字节流给服务器
        /// </summary>
        public void EnqueueSend(byte[] data)
        {
            if (_socket == null || _serverEndPoint == null || data == null)
                return;

            _sendQueue.Enqueue(data);
            _sendSignal.Release();
        }
        
        /// <summary>
        /// 异步发送Protobuf消息给服务器
        /// 自动序列化处理为字节流
        /// </summary>
        public void EnqueueSendProtobuf(BattleSyncRequest package)
        {
            if (package == null) return;
            
            byte[] data = package.ToByteArray();
            EnqueueSend(data);
        }

        #endregion
        
        #region 内部处理方法
        
        private async Task SendLoop(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    await _sendSignal.WaitAsync(token);

                    if (_sendQueue.TryDequeue(out var data))
                    {
                        await _socket.SendToAsync(
                            new ArraySegment<byte>(data),
                            SocketFlags.None,
                            _serverEndPoint
                        );
                    }
                }
                catch (ObjectDisposedException)
                {
                    // Socket 被 Close，正常退出
                    break;
                }
                catch (SocketException)
                {
                    // 网络异常
                    break;
                }
            }
        }
        
        private async Task ReceiveLoop(CancellationToken token)
        {
            byte[] buffer = new byte[BUFFER_SIZE];

            while (!token.IsCancellationRequested)
            {
                try
                {
                    SocketReceiveFromResult result =
                        await _socket.ReceiveFromAsync(
                            new ArraySegment<byte>(buffer),
                            SocketFlags.None,
                            _serverEndPoint
                        );

                    byte[] recv = new byte[result.ReceivedBytes];
                    Buffer.BlockCopy(buffer, 0, recv, 0, result.ReceivedBytes);

                    OnDataReceived?.Invoke(recv);
                }
                catch (ObjectDisposedException)
                {
                    // Socket 被 Close，正常退出
                    break;
                }
                catch (SocketException)
                {
                    // 网络异常
                    break;
                }
            }
        }
        
        #endregion
    }
}
