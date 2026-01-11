/* ------------------------------------------------------------
 *  Author:  2023051604032 WangXinKai
 *  Date:  2026.1.5
 *  LastUpdate:  2026.1.6
 * 
 *  功能简述：
 *  大厅界面房间列表的管理
 * 
 * ------------------------------------------------------------ */

using System;
using System.Numerics;
using LobbySyncPackage;
using UnityEngine;
using UnityEngine.UI;

namespace SceneUI.StartSceneUI
{
    public class RoomListManager:ListManager
    {
        public override void DeleteList()
        {
            base.DeleteList();
            
            
            //测试用
            // for (int index = 0; index != 4; index++)
            // {
            //     GameObject o=AddElement(index);
            //     o.GetComponent<RoomElementComponent>().SetRoomId((uint)index);
            // }
        }
        
        public void RefreshList(RefreshListResponsePackage response)
        {
            Debug.Log("RoomListManager:RefreshList"+response.Rooms.Count);
            if(!_preObject)Debug.Log("RoomListManager:RefreshList PreObject NULL");

            for (int i = 0; i < response.Rooms.Count; i++)
            {
                var room = response.Rooms[i];//创建游戏对象并在对应脚本添加数据
                
                GameObject o=AddElement(i);
                o.GetComponent<RoomElementComponent>().SetValue(room.RoomId, room.RoomName, room.RoomType, room.Owner,
                    room.Max, room.Num);
            }
        }

        public uint GetCurrentRoomId()
        {
            if (_elementByIndex.ContainsKey(_currentRoomIndex))
            {
                return _elementByIndex[_currentRoomIndex].GetComponent<RoomElementComponent>().GetRoomId();
            }
            else
            {
                return 0;
            }
        }
        
        public uint GetIdByIndex(int index)
        {
            if (_elementByIndex.ContainsKey(index))
            {
                return _elementByIndex[index].GetComponent<RoomElementComponent>().GetRoomId();
            }
            else
            {
                return 0;
            }
        }
    }
}