/* ------------------------------------------------------------
 *  Author:  2023051604044 wanrui
 *  Date:  2025.12.28
 *  LastUpdate:  2025.12.31
 *
 *  网络同步事件枚举
 * ------------------------------------------------------------ */

namespace Network
{
    /// <summary>
    /// 服务端发送回来的事件类型
    /// 对应与Protobuf-Response包中的各种枚举类型
    /// </summary>
    public enum NetEvent
    {
        ConnectResponse,    // ClientSyncPackage - RemoteClientEvent - ConnectResponse事件
        LoginResponse,
        LobbyRefresh,
        LobbyRoomInfo,
        LobbyRoomJoin
    }
}