/* ------------------------------------------------------------
 *  Author:  2023051604032 WangXinKai
 *  Date:  2026.1.7
 *  LastUpdate:  2026.1.7
 * 
 *  功能简述：
 *  房间界面
 * 
 * ------------------------------------------------------------ */

using System;
using EventProcess;
using LobbySyncPackage;
using Network;
using TMPro;
using UIFramework.Panel;
using UnityEngine;

namespace SceneUI.StartSceneUI
{
    public class RoomPressDownSignal : ASignal<RoomOption>{}

    public enum RoomOption
    {
        Exit,Start
    }
    
    public class RoomPanel:PanelController
    {
        [SerializeField] private TMP_Text _roomName;
        [SerializeField] private TMP_Text _roomId;
        [SerializeField] private TMP_Text _roomType;
        [SerializeField] private TMP_Text _roomDescription;
        
        [SerializeField] private PlayerListManager _playerListManagerA;
        [SerializeField] private PlayerListManager _playerListManagerB;
        
        public void UI_OnExitRoom()
        {
            //TODO:退出逻辑
            Signals.Get<StatusPromptWindowSignal>().Dispatch(true,"退出房间...");
        }

        public void UI_OnStartRoom()
        {
            //TODO:开始逻辑
            Signals.Get<StatusPromptWindowSignal>().Dispatch(true,"开始游戏...(暂无多人局内支持，后续功能待实现)");
        }
        
        /*
         * 回应包处理
         */
        public void RoomExit(RoomExitResponsePackage response)
        {
            Signals.Get<StatusPromptWindowSignal>().Dispatch(false,"");
            Signals.Get<RoomPressDownSignal>().Dispatch(RoomOption.Exit);
        }
        
        private void RoomEnter(RoomJoinResponsePackage response)
        {
            _playerListManagerA.DeleteList();
            _playerListManagerB.DeleteList();
            
            _roomName.text = response.RoomName;
            _roomId.text = response.RoomId.ToString();
            _roomType.text = response.RoomType;
            _roomDescription.text = response.RoomIntroduction;

            //TODO:处理房间玩家信息显示
            _playerListManagerA.RefreshList(response,0);
            _playerListManagerB.RefreshList(response,1);
        }
        
        
        protected override void SetProperties(PanelProperties props)
        {
            if (props is RoomPanelProperties properties)
            {
                RoomEnter(properties.Response);
            }
            base.SetProperties(props);
        }

        private void Start()
        {
            NetWorkManager.instance.RegisterEventHandler<RoomExitResponsePackage>(NetEvent.LobbyRoomExit,RoomExit);
        }
    }

    public class RoomPanelProperties : PanelProperties
    {
        public RoomJoinResponsePackage Response;

        public RoomPanelProperties(RoomJoinResponsePackage response):base(PanelPriority.None)
        {
            Response = response;
        }

    }
    
}