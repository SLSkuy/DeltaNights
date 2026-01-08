// SimpleTcpClient.cs
using UnityEngine;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class SimpleTcpClient : MonoBehaviour
{
    private TcpClient client;
    private Thread receiveThread;

    void Start()
    {
        ConnectToServer();

        // 测试：3秒后发送消息
        Invoke("TestSend", 3f);
    }

    void ConnectToServer()
    {
        try
        {
            client = new TcpClient();
            client.Connect("127.0.0.1", 12345);

            receiveThread = new Thread(Receive);
            receiveThread.Start();

            Debug.Log("Connected!");
        }
        catch (System.Exception e)
        {
            Debug.LogError(e.Message);
        }
    }

    void Receive()
    {
        byte[] buffer = new byte[1024];

        while (true)
        {
            try
            {
                int bytesRead = client.GetStream().Read(buffer, 0, buffer.Length);
                if (bytesRead > 0)
                {
                    string msg = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    Debug.Log("Server says: " + msg);
                }
            }
            catch
            {
                break;
            }
            Thread.Sleep(10);
        }
    }

    void TestSend()
    {
        string message = "Hello from Unity at " + Time.time;
        byte[] data = Encoding.UTF8.GetBytes(message);
        client.GetStream().Write(data, 0, data.Length);
        Debug.Log("Sent: " + message);
    }

    void OnDestroy()
    {
        if (receiveThread != null) receiveThread.Abort();
        if (client != null) client.Close();
    }
}