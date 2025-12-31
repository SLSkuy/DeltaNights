/* ------------------------------------------------------------
 *  Author:  2023051604044 wanrui
 *  Date:  2025.12.28
 *  LastUpdate:  2025.12.31
 *
 *  MessageProcesser负责序列化与反序列化所有接收到的网络字节流
 *  处理分发所有的网络事件
 *  管理所有的网路同步事件
 * ------------------------------------------------------------ */

using System;
using System.Collections.Generic;
using AckSyncPackage;
using BattleSyncPackage;
using Google.Protobuf;
using LobbySyncPackage;
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
            Debug.Log($"[MessageProcesser] Registered event {eventId}");
        }

        /// <summary>
        /// 注销所有Protobuf事件处理器
        /// </summary>
        /// <param name="eventId">事件ID</param>
        public void UnRegister(NetEvent eventId)
        {
            _handlers.Remove(eventId);
            Debug.Log($"[MessageProcesser] Unregistered event {eventId}");
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
                Debug.LogWarning($"[MessageProcessor] No handler for event {eventId}");
            }
        }

        #endregion

        #region 序列化处理

        /// <summary>
        /// 接受服务端发送的字符串，测试使用
        /// </summary>
        public void DeSerialize(byte[] data)
        {
            RemoteSyncPackage pkg = RemoteSyncPackage.Parser.ParseFrom(data);
            switch (pkg.EventID)
            {
                case RemoteSyncEvent.AckResponse:
                    AckResponseProcess(pkg.AckSync);
                    break;
                case RemoteSyncEvent.LobbyResponse:
                    LobbyResponseProcess(pkg.LobbyPackage);
                    break;
                case RemoteSyncEvent.BattleResponse:
                    BattleResponseProcess(pkg.BattlePackage);
                    break;
                default:
                    Debug.Log("Unknown EventID: " + pkg.EventID);
                    break;
            }
        }

        private void AckResponseProcess(AckSyncResponse response)
        {
            // TODO: 处理ACK回应消息，此处进行分发相应的网络事件
            switch (response.EventID)
            {
                case RemoteAckEvent.ConnectResponse:
                    Dispatch(NetEvent.ConnectResponse, response.ConnectResponse);
                    break;
                default:
                    break;
            }
        }

        private void LobbyResponseProcess(LobbySyncResponse response)
        {
            // TODO: 处理Lobby回应消息，此处进行分发相应的网络事件
            switch (response.EventID)
            {
                default:
                    break;
            }
        }

        private void BattleResponseProcess(BattleSyncResponse response)
        {
            // TODO: 处理Battle回应消息，此处进行分发相应的网络事件
            switch (response.EventID)
            {
                default:
                    break;
            }
        }

        #endregion
    }
}