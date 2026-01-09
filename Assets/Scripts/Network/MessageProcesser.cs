/* ------------------------------------------------------------
 *  Author:  2023051604044 wanrui
 *  Date:  2025.12.28
 *  LastUpdate:  2026.1.2
 *
 *  MessageProcesser负责序列化与反序列化所有接收到的网络字节流
 *  处理分发所有的网络事件
 *  管理所有的网路同步事件
 * ------------------------------------------------------------ */

using System;
using System.Collections.Generic;
using BattleSyncPackage;
using ClientSyncPackage;
using Google.Protobuf;
using LobbySyncPackage;
using Network.ProtoTools;
using SyncPackage;
using UnityEngine;

namespace Network
{
    public class MessageProcessor
    {
        private readonly Dictionary<NetEvent, Action<IMessage>> _handlers = new();

        #region 事件处理

        /// <summary>
        /// 注册Protobuf事件处理器
        /// </summary>
        /// <param name="eventId">事件ID</param>
        /// <param name="handler">事件</param>
        /// <typeparam name="T">Protobuf事件类型</typeparam>
        public void Register<T>(NetEvent eventId, Action<T> handler) where T : IMessage, new()
        {
            _handlers[eventId] = msg => handler((T)msg);
            Debug.Log($"[MessageProcesser] 注册事件 {eventId}");
        }

        /// <summary>
        /// 注销所有Protobuf事件处理器
        /// </summary>
        /// <param name="eventId">事件ID</param>
        public void UnRegister(NetEvent eventId)
        {
            _handlers.Remove(eventId);
            Debug.Log($"[MessageProcesser] 注销事件 {eventId}");
        }
        
        /// <summary>
        /// 分发Protobuf事件
        /// </summary>
        /// <param name="eventId">事件ID</param>
        /// <param name="message">Protobuf事件实例</param>
        private void Dispatch(NetEvent eventId, IMessage message)
        {
            if (_handlers.TryGetValue(eventId, out var handler))
            {
                handler(message);
            }
            else
            {
                Debug.LogWarning($"[MessageProcessor] 事件 {eventId} 没有对应的处理器");
            }
        }

        #endregion

        #region 序列化处理

        /// <summary>
        /// 接受服务端发送的TCP事件
        /// </summary>
        public void DeserializeTcp(byte[] data)
        {
            RemoteSyncPackage pkg = RemoteSyncPackage.Parser.ParseFrom(data);
            // ===== 分类处理网络同步包 =====
            switch (pkg.EventID)
            {
                case RemoteSyncEvent.ClientResponse:
                    AckResponseProcess(pkg.ClientPackage);
                    break;
                case RemoteSyncEvent.LobbyResponse:
                    LobbyResponseProcess(pkg.LobbyPackage);
                    break;
                default:
                    Debug.Log("[MessageProcesser] 未知事件: " + pkg.EventID);
                    break;
            }
        }

        /// <summary>
        /// 接收服务端发送的UDP事件
        /// </summary>
        /// <param name="data"></param>
        public void DeserializeUdp(byte[] data)
        {
            BattleSyncResponse pkg = ProtoPool.NewBattleResp();
            pkg.MergeFrom(data);
            
            // TODO: 处理战局同步回应包
            Debug.Log("接收到同步包");
            pkg.Dispose();
        }

        /// 处理Client类回应消息，此处进行分发相应的网络事件
        private void AckResponseProcess(ClientSyncResponse response)
        {
            switch (response.EventID)
            {
                case RemoteClientEvent.ConnectResponse:
                    Dispatch(NetEvent.ConnectResponse, response.ConnectResponse);
                    break;
                case RemoteClientEvent.LoginResponse:
                    Dispatch(NetEvent.LoginResponse, response.LoginResponse);
                    break;
                default:
                    break;
            }
        }

        /// 处理Lobby类回应消息，此处进行分发相应的网络事件
        private void LobbyResponseProcess(LobbySyncResponse response)
        {
            switch (response.EventID)
            {
                case RemoteLobbyEvent.RemoteLobbyRefresh:
                    Dispatch(NetEvent.LobbyRefresh,response.RefreshListResponse);
                    break;
                case RemoteLobbyEvent.RemoteLobbyRoomInfo:
                    Dispatch(NetEvent.LobbyRoomInfo, response.RefreshListResponse);
                    break;
                case RemoteLobbyEvent.RemoteLobbyRoomJoin:
                    Dispatch(NetEvent.LobbyRoomJoin, response.RoomJoinResponse);
                    break;
                case RemoteLobbyEvent.RemoteLobbyRoomExit:
                    Dispatch(NetEvent.LobbyRoomExit, response.RoomExitResponse);
                    break;
                default:
                    break;
            }
        }
        
        #endregion
    }
}