/* ------------------------------------------------------------
 *  Author:  2023051604044 wanrui
 *  Date:  2025.12.28
 *  LastUpdate:  2025.12.28
 *
 *  封装UDP
 *  负责处理所有的UDP网络传输事件
 *  实现可靠的UDP以保证服务器逻辑同步的实时性与可靠性
 * ------------------------------------------------------------ */

using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Network
{
    public class UdpManager
    {
        public event Action<byte[]> OnDataReceived;

        private Socket _socket;
        private EndPoint _serverEndPoint;
        private CancellationTokenSource _cts;

        private const int BUFFER_SIZE = 1024 * 64;

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
        }

        /// <summary>
        /// 发送UDP字节流给服务器
        /// </summary>
        public void Send(byte[] data)
        {
            if (_socket == null) return;

            if (_serverEndPoint == null) return;
            _socket.SendTo(data, _serverEndPoint);
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

        public void Stop()
        {
            _cts?.Cancel();
            _socket?.Close();
            _socket = null;
        }
    }
}
