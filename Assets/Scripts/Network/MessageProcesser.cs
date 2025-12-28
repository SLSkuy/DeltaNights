/* ------------------------------------------------------------
 *  Author:  2023051604044 wanrui
 *  Date:  2025.12.28
 *  LastUpdate:  2025.12.28
 *
 *  MessageProcesser负责序列化与反序列化所有接收到的网络字节流
 *  处理分发所有的网络事件
 *  管理所有的网路同步事件
 * ------------------------------------------------------------ */

using System;
using System.Collections.Generic;
using System.Text;
using Google.Protobuf;
using UnityEngine;

namespace Network
{
    public class MessageProcessor
    {
        private readonly Dictionary<uint, Action<IMessage>> _handlers = new();

        /// <summary>
        /// 注册Protobuf事件
        /// </summary>
        /// <param name="msgId">事件ID</param>
        /// <param name="handler">事件</param>
        /// <typeparam name="T">事件参数</typeparam>
        public void Register<T>(uint msgId, Action<T> handler) where T : IMessage, new()
        {
            _handlers[msgId] = msg => handler((T)msg);
        }

        /// <summary>
        /// 将字节流转换为Protobuf事件
        /// </summary>
        public void DeSerialize(byte[] data)
        {
            // TODO: 反序列化Protobuf消息
            Debug.Log(Encoding.UTF8.GetString(data));
        }

        /// <summary>
        /// 将Protobuf事件序列化为字节流
        /// </summary>
        public void Serialize()
        {
            
        }
    }
}