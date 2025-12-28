/* ------------------------------------------------------------
 *  Author:  2023051604044 wanrui
 *  Date:  2025.12.28
 *  LastUpdate:  2025.12.28
 *
 *  封装TCP
 *  负责处理所有的TCP网络传输事件
 *  只负责TCP消息的发送与接受
 * ------------------------------------------------------------ */

using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Network
{
    public class TcpManager
    {
        public event Action<byte[]> OnMessageReceived;
        public event Action OnDisconnected;

        private Socket _socket;
        private CancellationTokenSource _cts;

        public bool Connected => _socket != null && _socket.Connected;

        /// <summary>
        /// 连接到服务器
        /// </summary>
        /// <param name="serverIp">服务器IP地址</param>
        /// <param name="serverPort">服务器端口</param>
        public void Connect(string serverIp, short serverPort)
        {
            Disconnect();

            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                _socket.Connect(IPAddress.Parse(serverIp), serverPort);
                _cts = new CancellationTokenSource();
                _ = Task.Run(() => ReceiveLoop(_cts.Token));
            }
            catch (Exception e)
            {
                throw new Exception($"TCP Connect Failed: {e.Message}");
            }
        }

        /// <summary>
        /// 发送TCP字节流给服务器
        /// </summary>
        public void Send(byte[] data)
        {
            if (!Connected) return;
            _socket.Send(data);
        }

        private async Task ReceiveLoop(CancellationToken token)
        {
            byte[] buffer = new byte[1024 * 1024];

            while (!token.IsCancellationRequested)
            {
                try
                {
                    int len = await _socket.ReceiveAsync(buffer, SocketFlags.None, token);
                    if (len <= 0)
                        break;
                    
                    OnMessageReceived?.Invoke(buffer);
                }
                catch
                {
                    break;
                }
            }

            OnDisconnected?.Invoke();
        }

        public void Disconnect()
        {
            _cts?.Cancel();

            if (_socket == null) return;

            if (_socket.Connected)
                _socket.Shutdown(SocketShutdown.Both);

            _socket.Close();
            _socket = null;
        }
    }
}
