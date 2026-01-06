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
    public class RoomCellComponent:MonoBehaviour
    {
        private int _index;//外界所用的索引
        private uint _roomId;
        private string _roomName;
        private string _roomType;
        private string _owner;
        private int _max;
        private int _num;

        private void Start()
        {
            
        }

        public void SetValue(uint roomId,string roomName,string roomType,string owner,int max,int num)
        {
            _roomId = roomId;
            _roomName = roomName;
            _roomType = roomType;
            _owner = owner;
            _max = max;
            _num = num;
        }

        public void SetIndex(int index)
        {
            _index = index;
        }
    }
}