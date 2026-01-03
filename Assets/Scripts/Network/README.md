# 服务器同步事件处理

## TCP事件处理流程
在```SyncPackage.proto```文件中，定义了TCP同步事件的类型

同时每一大类下细分了多个小类
* ```ClientSyncPackage.proto```：客户端的各种事件（登录、连接请求等）
* ```LobbySyncPackage.proto```：大厅的各种事件（组队、背包、商店等）

每一类事件都分为请求包与回应包
* 请求包由客户端发送，服务端接收并处理
* 回应包由服务端发送，客户端接收并处理

### 客户端请求事件示例
以绑定UDP端口为例，首先客户端构造一个请求包
``````C#
LocalSyncPackage syncPackage = new LocalSyncPackage
{
    EventID = LocalSyncEvent.ClientRequest;     // 指定包大类
    ClientSync = new ClientSyncRequest 
    {
        EventID = LocalClientEvent.ConnectRequest,  // 指定包小类
        Connect = new ConnectRequestPackage
        {
            Port = _udp.UdpPort // 填入对应信息
        }
    };
}
``````
通过调用```NetworkManager.SendTcp(LocalSyncPackage pkg)```方法即可自动处理并将消息发送给服务端

服务端的```TcpEndpoint```类将接收并处理接收到的字节流，并将序列化后的结果传递给```NetworkDispatcher```对象，该对象将根据包内的事件ID进行事件的分发
```C++
// 大类识别
void NetworkDispatcher::handleTcpPackage(QTcpSocket* socket, const QByteArray& data)
{
    using namespace SyncPackage;

    LocalSyncPackage pkg;
    if (!pkg.ParseFromArray(data.constData(), data.size()))
    {
        // 错误处理
    }

    // ===== 按类型分发 =====
    switch (pkg.eventid())
    {
        case LocalSyncEvent::ClientRequest:
            handleTcpAckPackage(socket, pkg.clientsync());
            break;
        // 其余类型...
    }
}

// 子类分发
void NetworkDispatcher::handleTcpAckPackage(QTcpSocket* socket, const ClientSyncPackage::ClientSyncRequest& pkg)
{
    using namespace ClientSyncPackage;

    // ===== 按子类型分发 =====
    switch (pkg.eventid())
    {
        case LocalClientEvent::ConnectRequest:
            // TODO: 客户端连接请求
            emit clientBindUdpPort(socket, pkg.connect().port());
            break;
        // 其余类型...
    }
}
```
```NetworkDispatcher```对象将触发对应的信号，接着只需由相应的处理对象接收信号并处理即可

这里连接信号的是```ClientManager```对象，它将处理对应的事件
```C++
void GameServer::setupConnections()
{
    // TODO: 信号连接
    // 这一行连接触发的信号
    connect(_dispatcher,&NetworkDispatcher::clientBindUdpPort,_clientMgr,&ClientManager::clientBindUdpPort);
    // ...
}

// 信号处理
void ClientManager::clientBindUdpPort(QTcpSocket* socket, quint16 port)
{
    // TODO: 信号处理
}
```

### 服务端回应事件示例
以回应连接请求为例，同样是先构造回应包
```C++
// 创建回复Protobuf包
using namespace SyncPackage;
RemoteSyncPackage response;
response.set_eventid(RemoteSyncEvent::ClientResponse);
auto* type = response.mutable_clientpackage();
type->set_eventid(ClientSyncPackage::RemoteClientEvent::ConnectResponse);
auto* connectResponsePkg = type->mutable_connectresponse();
connectResponsePkg->set_content(QString("服务器连接成功").toStdString());

// 触发连接回复信号
emit clientConnectResponse(socket, response);
```
然后委托```NetworkDispatcher```进行回应消息的发送，这里则是通过信号连接的形式，直接触发信号
```C++
void GameServer::setupConnections()
{
    // TODO: 信号连接
    // 这一行连接触发的信号
    connect(_clientMgr,&ClientManager::clientConnectResponse,_dispatcher,&NetworkDispatcher::sendTcpMessage);
    // 一定是连接的sendTcpMessage函数
    // ...
}
```
```NetworkDispatcher```会自动处理好TCP消息的序列化等操作，并委托```TcpEndpoint```进行消息的发送

客户端的```TcpManager```将接收到服务端发送来的字节流，它会将字节流委托给```MessageProcessor```进行处理

```MessageProcessor```首先会反序列化字节流，同时判断事件类型，进行事件的分发
```C#
public void DeserializeTcp(byte[] data)
{
    RemoteSyncPackage pkg = RemoteSyncPackage.Parser.ParseFrom(data);
    // ===== 分类处理网络同步包 =====
    switch (pkg.EventID)
    {
        case RemoteSyncEvent.ClientResponse:
            AckResponseProcess(pkg.ClientPackage);
            break;
        // ...
    }
}

private void AckResponseProcess(ClientSyncResponse response)
{
    switch (response.EventID)
    {
        case RemoteClientEvent.ConnectResponse:
            Dispatch(NetEvent.ConnectResponse, response.ConnectResponse);
            break;
        // ...
    }
}
```
```Dispatch```方法会从已经注册了相应处理函数的类型中查找并调用相应的处理函数进行事件的处理，因此每一个类型都需要有一个对应的处理函数
```C#
void Start()
{
    // 注册事件处理器
    NetWorkManager.instance.RegisterEventHandler<ConnectResponsePackage>(
        NetEvent.ConnectResponse
        , PrintConnectResponse);
}

// 事件处理函数
void PrintConnectResponse(ConnectResponsePackage package)
{
    Debug.Log($"Connect response package {package.Content}");
}
```

## TCP事件的拓展
要拓展TCP事件，首先要在```.proto```文件中，添加对应的类型，指定所在的小类，创建相应的请求类型与回应类型，并生成对应的 C# 与 C++ 代码，
```protobuf
syntax = "proto3";

enum LocalClientEvent {
  // 添加新类型请求枚举
  xxxReq = 0;
}

// ===== ACK请求包 =====
message ClientSyncRequest {
  LocalClientEvent eventID = 1;
  oneof content {
    // 添加新类型请求包
    xxxRequestPackage xxx = 2;
  }
}

// 添加新类型请求包定义
message xxxRequestPackage{
  // 属性
}

enum RemoteClientEvent {
  // 添加新类型回应枚举
  xxxReps = 0;
}

// ===== ACK回应包 =====
message ClientSyncResponse {
  RemoteClientEvent eventID = 1;
  oneof content {
    // 添加新类型包
    xxxResponsePackage xxx = 2;
  }
}

// 添加新类型回应包定义
message xxxResponsePackage {
  // 属性
}
```
对于客户端，需要拓展```NetEvent```枚举，同时给对应事件注册相应的处理函数
```C#
namespace Network
{
    // 这里的枚举只在注册事件处理函数时使用
    // 指明不同的事件类型，并非Protobuf中的各类型包中的枚举
    public enum NetEvent
    {
        ConnectResponse,    // 对应ClientSyncPackage - RemoteClientEvent - ConnectResponse事件
        // 拓展枚举
        xxx
    }
}

// 注册处理函数
NetWorkManager.instance.RegisterEventHandler<ConnectResponsePackage>(NetEvent.xxx , HandleFunction);
```
还要在```MessageProcessor```中添加新的枚举类识别处理逻辑
```C#
/// 处理Client类回应消息，此处进行分发相应的网络事件
private void AckResponseProcess(ClientSyncResponse response)
{
    switch (response.EventID)
    {
        case RemoteClientEvent.ConnectResponse:
            Dispatch(NetEvent.ConnectResponse, response.ConnectResponse);
            break;
        // 添加新的枚举类型处理
    }
}
```
对于服务器，需要在```NetworkDispatcher```中添加新的枚举类型处理，创建新的信号，以及新的处理函数
```C++
void NetworkDispatcher::handleTcpClientPackage(QTcpSocket* socket, const ClientSyncPackage::ClientSyncRequest& pkg)
{
    using namespace ClientSyncPackage;

    // ===== 按子类型分发 =====
    switch (pkg.eventid())
    {
        case LocalClientEvent::HeartBeat:
            // TODO: 心跳消息处理
            emit clientHeartBeat(socket);
            break;
        case LocalClientEvent::ConnectRequest:
            // TODO: 客户端连接请求
            emit clientBindUdpPort(socket, pkg.connect().port());
            break;
        // 添加新的枚举类型处理
    }
}
```