/* ------------------------------------------------------------
 *  Author:  2023051604032 WangXinKai
 *  Date:  2025.12.23
 *  LastUpdate:  2025.12.23
 *
 *  功能简述：
 *  游戏开始界面的大厅界面
 *  仅显示在准备状态的房间
 *
 * ------------------------------------------------------------ */

using EventProcess;
using TMPro;
using UIFramework.Panel;
using UnityEngine;
using LobbySyncPackage;
using Network;
using SyncPackage;

namespace SceneUI.StartSceneUI
{
    public class HallPressDownSignal : ASignal<HallOption>{}
    
    public enum HallOption
    {
        BackToMain,JoinRoom,CreateRoom
    }
    public class HallPanel: PanelController
    {
        [SerializeField] RoomListManager _roomListManager;
        [SerializeField] PlayerListManager _playerListManagerA;
        [SerializeField] PlayerListManager _playerListManagerB;
        //字体自动更换
        [SerializeField] private TMP_FontAsset font;
        void OnEnable()
        {
            if (font!= null)
            {
                // 遍历所有子物体（包括自己）的TMP_Text组件
                TMP_Text[] allTexts = GetComponentsInChildren<TMP_Text>(true);
                foreach (TMP_Text text in allTexts)
                {
                    text.font = font;
                }
            }
            
            UI_RefreshList();
        }
        
        /*
         * 接收和处理回应
         */
        
        //用于处理事件-大厅列表包
        void RefreshList(RefreshListResponsePackage response)
        {
            //TODO:对接 接收大厅列表
            Debug.Log("RefreshList:"+response);
            _roomListManager.RefreshList(response);
        }

        void RefreshRoomInfo(RoomInfoResponsePackage response)
        {
            //TODO:对接 大厅房间信息
            _playerListManagerA.RefreshList(response,0);
            _playerListManagerB.RefreshList(response,1);
            
        }
        
        /*
         * 请求发送
         */
        public void UI_RefreshList()
        {
            _roomListManager.DeleteList();
            //TODO:发送列表请求
            LocalSyncPackage syncPackage = new LocalSyncPackage
            {
                EventID = LocalSyncEvent.LobbyRequest,
                LobbySync = new LobbySyncRequest
                {
                    EventID = LocalLobbyEvent.LocalLobbyRefresh 
                }
            };
            //TODO:对接 发送刷新请求
            NetWorkManager.instance.SendTcp(syncPackage);
            Debug.Log("请求刷新");
        }

        public void UI_JoinRoom()
        {
            //TODO:加入房间
            Debug.Log("加入房间"+_roomListManager._currentRoomIndex);
            LocalSyncPackage syncPackage = new LocalSyncPackage
            {
                EventID = LocalSyncEvent.LobbyRequest,
                LobbySync = new LobbySyncRequest
                {
                    EventID = LocalLobbyEvent.LocalLobbyRoomJoin,
                    RoomJoin = new RoomJoinRequest
                    {
                        RoomId = _roomListManager.GetCurrentRoomId()
                    }
                }
            };
            NetWorkManager.instance.SendTcp(syncPackage);
            Debug.Log("请求加入");
            Signals.Get<StatusPromptWindowSignal>().Dispatch(true,"加入房间 "+_roomListManager.GetCurrentRoomId()+" 中...");
        }

        public void UI_CreateRoom()
        {
            //TODO:创建房间
            Signals.Get<HallPressDownSignal>().Dispatch(HallOption.CreateRoom);
        }
        
        
        public void UI_OnBackButtonPressDown()
        {
            Signals.Get<HallPressDownSignal>().Dispatch(HallOption.BackToMain);
        }

        void Start()
        {
            NetWorkManager.instance.RegisterEventHandler<RefreshListResponsePackage>(NetEvent.LobbyRefresh,RefreshList);
            NetWorkManager.instance.RegisterEventHandler<RoomInfoResponsePackage>(NetEvent.LobbyRoomInfo,RefreshRoomInfo);
        }
    }
}