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
using TMPro;
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
        [SerializeField] private TMP_Text _roomNameText;
        [SerializeField] private TMP_Text _roomTypeText;
        [SerializeField] private TMP_Text _ownerText;
        [SerializeField] private TMP_Text _numText;

        public void SetValue(uint roomId,string roomName,string roomType,string owner,int max,int num)
        {
            _roomId = roomId;
            _roomName = roomName;
            _roomType = roomType;
            _owner = owner;
            _max = max;
            _num = num;
            _roomNameText.text = roomName ?? "未知名称";
            _roomTypeText.text = roomType ?? "未知类型";
            _ownerText.text = owner ?? "未知所有者";
            if (max != 0 )
            {
                _numText.text = num.ToString()+"/"+max.ToString();
            }
            else
            {
                _numText.text = "未知人数";
            }
        }

        public void SetRoomId(uint roomId)
        {
            _roomId = roomId;
        }

        public uint GetRoomId()
        {
            return _roomId;
        }
    }
}