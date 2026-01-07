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
    public class RoomListManager:MonoBehaviour
    {
        [SerializeField] private GameObject roomCell;
        public int _currentRoomIndex{get;private set;}//外界只读 当前的房间焦点

        private void Awake()
        {
        }

        public void DeleteList()
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                Destroy(transform.GetChild(i).gameObject);
            }
            
            //TODO:测试用
            for (int i = 0; i != 4; i++)
            {
                GameObject o = AddRoomElement(i);
            }
        }

        public void RefreshList(RefreshListResponsePackage response)
        {

            for (int i = 0; i < response.Rooms.Count; i++)
            {
                var room = response.Rooms[i];//创建游戏对象并在对应脚本添加数据
                
                GameObject o=AddRoomElement(i);
                o.GetComponent<RoomCellComponent>().SetValue(room.RoomId, room.RoomName, room.RoomType, room.Owner,
                    room.Max, room.Num);
            }
        }
    
        //添加对应的房间元素
        private GameObject AddRoomElement(int index)
        {
            GameObject o=Instantiate(roomCell,this.transform);
            o.GetComponent<RoomCellComponent>().SetIndex(index);
            Button b=o.GetComponent<Button>();
            b.onClick.AddListener(() =>
            {
                OnElementClick(index);
                //TODO:请求玩家列表
            });
            return o;
        }

        void OnElementClick(int index)
        {
            Debug.Log("点击"+index);
            _currentRoomIndex=index;
        }
    }
}