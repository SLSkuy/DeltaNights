using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class SocketTest : MonoBehaviour
{
    private Socket _socket;
    private Thread _receiveThread;
    private bool _isRunning = false;
    
    public InputField inputField;

    void Start()
    {
        _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        _socket.Connect(IPAddress.Parse("127.0.0.1"), 11451);

        Debug.Log("连接成功");
        _socket.Send(Encoding.UTF8.GetBytes("客户端连接"));

        _isRunning = true;
        _receiveThread = new Thread(ReceiveLoop)
        {
            IsBackground = true // 后台线程
        };
        _receiveThread.Start();
        
        inputField.onSubmit.AddListener(SendToServer);
    }

    private void SendToServer(string text)
    {
        if (_socket.Connected)
        {
            _socket.Send(Encoding.UTF8.GetBytes(text));    
        }
    }

    private void ReceiveLoop()
    {
        byte[] buffer = new byte[1024];
        while (_isRunning)
        {
            try
            {
                int length = _socket.Receive(buffer);
                if (length > 0)
                {
                    string msg = Encoding.UTF8.GetString(buffer, 0, length);
                    Debug.Log("收到消息: " + msg);
                }
            }
            catch (SocketException e)
            {
                Debug.LogWarning("Socket异常: " + e.Message);
                break;
            }
        }
    }

    void OnDestroy()
    {
        _isRunning = false;
        try
        {
            _socket.Shutdown(SocketShutdown.Both);
            _socket?.Close();
        } catch { }
        _receiveThread?.Join();
    }
}