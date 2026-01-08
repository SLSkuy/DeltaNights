/* ------------------------------------------------------------
 *  Author:  2023051604032 WangXinKai
 *  Date:  2026.1.5
 *  LastUpdate:  2026.1.6
 * 
 *  功能简述：
 *  大厅界面房间列表的每个元素的功能实现
 * 
 * ------------------------------------------------------------ */

using System;
using UnityEngine;

namespace SceneUI.StartSceneUI
{
    public class RoomElementComponent:ListElement
    {
        private uint _roomId=0;
        private string _roomName;
        private string _roomType;
        private string _owner;
        private int _max;
        private int _num;

        public void SetValue(uint roomId,string roomName,string roomType,string owner,int max,int num)
        {
            _roomId = roomId;
            _roomName = roomName;
            _roomType = roomType;
            _owner = owner;
            _max = max;
            _num = num;
        }

        public uint GetRoomId()
        {
            return _roomId;
        }
    }
}