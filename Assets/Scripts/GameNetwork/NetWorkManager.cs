using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace GameNetwork
{
    public class NetWorkManager : MonoBehaviour
    {
        public static NetWorkManager Instance;

        public event Action<string> ReceiveMessage;
        
        private Socket _socket;
        private CancellationTokenSource _cts;
        
        private string _ip;
        private short _port;

        private Queue<string> _receiveMsgs;
        
        
        void Awake()
        {
            _receiveMsgs = new Queue<string>();
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Instance = this;
            
            DontDestroyOnLoad(gameObject);
            
            ConnectToServer("127.0.0.1",11451);
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

        public void ConnectToServer(string ip, short port)
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
            if (_socket != null && _socket.Connected)
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
                    int len = await _socket.ReceiveAsync(msgBytes, SocketFlags.None, token);
                    if (len <= 0)
                    {
                        Debug.LogWarning("Server Closed");
                        break;
                    }

                    string msg = Encoding.UTF8.GetString(msgBytes, 0, len);
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

        void OnDestroy()
        {
            _cts?.Cancel();
            
            try
            {
                if (_socket != null && _socket.Connected)
                {
                    _socket.Shutdown(SocketShutdown.Both);
                    _socket.Close();
                }
            }catch(Exception e)
            {
                Debug.LogError(e);
            }
        }
    }
}
