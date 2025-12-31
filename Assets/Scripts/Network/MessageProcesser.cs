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
using System.Text;
using Google.Protobuf;
using SyncPackage;
using UnityEngine;

namespace Network
{
    public class MessageProcessor
    {
        private readonly Dictionary<NetEvent, Action<IMessage>> _handlers = new();

        /// <summary>
        /// 注册Protobuf事件
        /// </summary>
        /// <param name="eventId">事件ID</param>
        /// <param name="handler">事件</param>
        /// <typeparam name="T">事件参数</typeparam>
        public void Register<T>(NetEvent eventId, Action<T> handler) where T : IMessage, new()
        {
            _handlers[eventId] = msg => handler((T)msg);
        }

        /// <summary>
        /// 将字节流转换为Protobuf事件
        /// </summary>
        public RemoteSyncPackage DeSerializeProtobuf(byte[] data)
        {
            RemoteSyncPackage syncPackage = RemoteSyncPackage.Parser.ParseFrom(data);
            return syncPackage;
        }
        
        /// <summary>
        /// 将Protobuf事件序列化为字节流
        /// </summary>
        public byte[] SerializeProtobuf(LocalSyncPackage syncPackage)
        {
            return syncPackage.ToByteArray();
        }

        /// <summary>
        /// 接受服务端发送的字符串，测试使用
        /// </summary>
        public void DeSerialize(byte[] data)
        {
            RemoteSyncPackage pkg = RemoteSyncPackage.Parser.ParseFrom(data);
            switch (pkg.EventID)
            {
                case RemoteSyncEvent.AckResponse:
                    Debug.Log("Ack Response");
                    break;
                case RemoteSyncEvent.BattleResponse:
                    Debug.Log("Battle Response");
                    break;
                case RemoteSyncEvent.LobbyResponse:
                    Debug.Log("Lobby Response");
                    break;
                default:
                    Debug.Log("Unknown EventID: " + pkg.EventID);
                    break;
            }
        }
    }
}