/* ------------------------------------------------------------
 *  Author:  2023051604044 wanrui
 *  Date:  2025.12.28
 *  LastUpdate:  2025.12.30
 *
 *  封装TCP
 *  负责处理所有的TCP网络传输事件
 *  只负责TCP消息的发送与接受
 * ------------------------------------------------------------ */

using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Google.Protobuf;
using SyncPackage;
using UnityEngine;

namespace Network
{
    public class TcpManager
    {
        private Socket _socket;
        private CancellationTokenSource _cts;
        public bool Connected => _socket is { Connected: true };
        
        private const int BUFFER_SIZE = 1024 * 1024;
        
        // 黏包处理
        private readonly byte[] _receiveBuffer = new byte[BUFFER_SIZE];
        private int _receiveCount = 0;
        
        // 异步发送队列
        private readonly ConcurrentQueue<byte[]> _sendQueue = new();
        private readonly SemaphoreSlim _sendSignal = new(0);

        #region 事件

        public event Action<byte[]> OnMessageReceived;
        public event Action OnDisconnected;

        #endregion
        
        #region TCP控制
        
        /// <summary>
        /// 连接到服务器
        /// </summary>
        /// <param name="serverIp">服务器IP地址</param>
        /// <param name="serverPort">服务器端口</param>
        public void Connect(string serverIp, short serverPort)
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                _socket.Connect(IPAddress.Parse(serverIp), serverPort);
                _cts = new CancellationTokenSource();
                _ = Task.Run(() => ReceiveLoop(_cts.Token));
                _ = Task.Run(() => SendLoop(_cts.Token));;
            }
            catch (Exception e)
            {
                throw new Exception($"TCP Connect Failed: {e.Message}");
            }
        }

        public void Disconnect()
        {
            _cts?.Cancel();

            _sendSignal?.Release();

            if (_socket == null) return;

            if (_socket.Connected)
                _socket.Shutdown(SocketShutdown.Both);

            _socket.Close();
            _socket = null;
        }

        /// <summary>
        /// 异步发送TCP字节流给服务器
        /// </summary>
        public void EnqueueSend(byte[] data)
        {
            if (!Connected || data == null) return;

            int len = data.Length;

            // 统一使用大端序列
            byte[] length = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(len));

            byte[] packet = new byte[length.Length + data.Length];
            Buffer.BlockCopy(length, 0, packet, 0, length.Length);
            Buffer.BlockCopy(data, 0, packet, length.Length, data.Length);

            _sendQueue.Enqueue(packet);
            _sendSignal.Release();
        }


        /// <summary>
        /// 异步发送Protobuf消息给服务器
        /// 自动序列化处理为字节流
        /// </summary>
        public void EnqueueSendProtobuf(LocalSyncPackage package)
        {
            if (!Connected) return;
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
                        await _socket.SendAsync(data, SocketFlags.None, token);
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
                    int len = await _socket.ReceiveAsync(buffer, SocketFlags.None, token);
                    if (len <= 0)
                        break;
                    
                    Array.Copy(buffer, 0, _receiveBuffer, _receiveCount, len);
                    _receiveCount += len;
                    
                    ProcessBuffer();
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

            OnDisconnected?.Invoke();
        }

        private void ProcessBuffer()
        {
            int offset = 0;

            while (true)
            {
                // 至少要有 4 字节长度头
                if (_receiveCount - offset < 4)
                    break;

                int bodyLength = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(_receiveBuffer, offset));

                if (bodyLength is <= 0 or > 10 * 1024 * 1024)
                {
                    // 非法包，直接清空
                    _receiveCount = 0;
                    break;
                }

                // 数据不完整，等待下次接收
                if (_receiveCount - offset - 4 < bodyLength)
                    break;

                // 读取完整消息
                byte[] msg = new byte[bodyLength];
                Array.Copy(_receiveBuffer, offset + 4, msg, 0, bodyLength);

                OnMessageReceived?.Invoke(msg);

                // 移动偏移量
                offset += 4 + bodyLength;
            }

            // 把剩余未处理的数据前移
            if (offset > 0)
            {
                Array.Copy(_receiveBuffer, offset, _receiveBuffer, 0, _receiveCount - offset);
                _receiveCount -= offset;
            }
        }
        
        #endregion
    }
}
