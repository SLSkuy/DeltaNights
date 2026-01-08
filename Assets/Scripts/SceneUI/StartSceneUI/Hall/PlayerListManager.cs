using System;
using LobbySyncPackage;
using UnityEngine;

namespace SceneUI.StartSceneUI
{
    public class PlayerListManager:ListManager
    {
        public override void DeleteList()
        {
            base.DeleteList();
            Debug.Log("PlayerListManager:DeleteList");
            
            //测试用
            for (int index = 0; index != 4; index++)
            {
                this.AddElement(index);
            }
        }

        private void Awake()
        {
            if (!_preObject)
            {
                Debug.Log("PlayerListManager:Awake 空预制体");
            }
            DeleteList();
        }
        
        protected override GameObject AddElement(int index)
        {
            GameObject o=Instantiate(_preObject,this.transform);
            o.GetComponent<ListElement>().SetIndex(index);
            
            _elementByIndex.Add(index, o);
            
            return o;
        }
        
        public void RefreshList(RoomInfoResponsePackage response,int index)
        {
            if (index == 0)
            {
                for (int i = 0; i < response.TeamAPlayers.Count; i++)
                {
                    //创建游戏对象并在对应脚本添加数据
                    GameObject o=AddElement(i);
                    o.GetComponent<PlayerElementComponent>().SetValue(response.TeamAPlayers[i]);
                }
            }else if (index == 1)
            {
                for (int i = 0; i < response.TeamBPlayers.Count; i++)
                {
                    //创建游戏对象并在对应脚本添加数据
                    GameObject o=AddElement(i);
                    o.GetComponent<PlayerElementComponent>().SetValue(response.TeamBPlayers[i]);
                }
            }
            
        }
    }
}