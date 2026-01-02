/* ------------------------------------------------------------
 *  Author:  2023051604044 wanrui
 *           2023051604032 WangXinKai
 *  Date:  2025.12.31
 *  LastUpdate:  2026.1.2
 *
 *  负责Protobuf对象的快捷创建
 * ------------------------------------------------------------ */

using System;
using AckSyncPackage;
using BattleSyncPackage;
using LobbySyncPackage;
using SyncPackage;

namespace Network.ProtoTools
{
    /// <summary>
    /// Protobuf对象创建工具
    /// 负责快捷创建Protobuf对象
    /// </summary>
    public static class ProtoBuilder
    {
        
        /// <summary>
        ///  战局同步
        /// </summary>
        public static class Battle
        {
            /// <summary>
            ///  客户端->服务端同步包-本地战局同步请求包BattleSyncRequest(快捷创建Protobuf对象)
            /// </summary>
            public static LocalSyncPackage BuildRequest(LocalBattleEvent battleEventID )//TODO:内容呢
            {
                return new LocalSyncPackage
                {
                    EventID = LocalSyncEvent.AckRequest,
                    BattleSync = new BattleSyncRequest
                    {
                        EventID = battleEventID
                    }
                };
            }
            
            /// <summary>
            ///  服务端->客户端同步包-远程战局同步回应包BattleSyncResponse(快捷创建Protobuf对象)
            /// </summary>
            public static RemoteSyncPackage BuildResponse(RemoteBattleEvent battleEventID)
            {
                return new RemoteSyncPackage
                {
                    EventID = RemoteSyncEvent.BattleResponse,
                    BattlePackage = new BattleSyncResponse
                    {
                        EventID = battleEventID
                    }
                };
            }
        }
        
        /// <summary>
        ///  大厅操作同步
        /// </summary>
        public static class Lobby
        {
            /// <summary>
            /// 客户端->服务端同步包-大厅操作请求包LobbySyncRequest(快捷创建Protobuf对象)
            /// </summary>
            public static LocalSyncPackage BuildRequest(LocalLobbyEvent lobbyEventID)
            {
                return new LocalSyncPackage
                {
                    EventID = LocalSyncEvent.LobbyRequest,
                    LobbySync = new LobbySyncRequest
                    {
                        EventID = lobbyEventID
                    }
                };
            }
            
            /// <summary>
            /// 服务端->客户端同步包-大厅操作回应包LobbySyncResponse(快捷创建Protobuf对象)
            /// </summary>
            public static RemoteSyncPackage BuildResponse(RemoteLobbyEvent lobbyEventID)
            {
                return new RemoteSyncPackage
                {
                    EventID = RemoteSyncEvent.LobbyResponse,
                    LobbyPackage = new LobbySyncResponse
                    {
                        EventID = lobbyEventID
                    }
                };
            }
        }
        
        /// <summary>
        /// ACK包
        /// </summary>
        public static class ACK
        {
            /// <summary>
            /// 客户端->服务端同步包-ACK请求包AckSyncRequest-事件包HeartBeatPackage(快捷创建Protobuf对象)
            /// </summary>
            public static LocalSyncPackage BuildHeartBeat(UInt32 clientId)
            {
                return new LocalSyncPackage
                {
                    EventID = LocalSyncEvent.AckRequest,
                    AckSync =new AckSyncRequest
                    {
                        EventID = LocalAckEvent.HeartBeat,
                        HeartBeat = new HeartBeatPackage
                        {
                            ClientID = clientId
                        }
                    }
                };
            }
        
            /// <summary>
            /// 客户端->服务端同步包-ACK请求包AckSyncRequest-连接请求包ConnectRequestPackage(快捷创建Protobuf对象)
            /// </summary>
            public static LocalSyncPackage BuildConnectRequest(int port)
            {
                return new LocalSyncPackage
                {
                    EventID = LocalSyncEvent.AckRequest,
                    AckSync =new AckSyncRequest
                    {
                        EventID = LocalAckEvent.ConnectRequest,
                        Connect = new ConnectRequestPackage
                        {
                            Port = port
                        }
                    }
                };
            }
            
            /// <summary>
            /// 服务端->客户端同步包-ACK回应包AckSyncResponse-连接回应包ConnectResponsePackage(快捷创建Protobuf对象)
            /// </summary>
            public static RemoteSyncPackage BuildConnectResponse(String content)
            {
                return new RemoteSyncPackage
                {
                    EventID = RemoteSyncEvent.AckResponse,
                    AckSync = new AckSyncResponse
                    {
                        EventID = RemoteAckEvent.ConnectResponse,
                        ConnectResponse = new ConnectResponsePackage
                        {
                            Content = content
                        }
                    }
                };
            }
        }
    }
}