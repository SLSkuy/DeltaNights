/* ------------------------------------------------------------
 *  Author:  2023051604032 WangXinKai
 *  Date:  2025.12.23
 *  LastUpdate:  2025.12.23
 *
 *  功能简述：
 *  游戏开始界面的大厅界面
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
    public class BackToMainPressDownSignal : ASignal{}
    public class HallPanel: PanelController
    {
        [SerializeField] RoomListManager roomListManager;
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

        //用于处理事件-大厅列表包
        void RefreshList(RefreshListResponsePackage response)
        {
            roomListManager.RefreshList(response);
        }

        public void UI_RefreshList()
        {
            roomListManager.DeleteList();
            //TODO:发送列表请求
            LocalSyncPackage syncPackage = new LocalSyncPackage
            {
                EventID = LocalSyncEvent.LobbyRequest,
                LobbySync = new LobbySyncRequest
                {
                    EventID = LocalLobbyEvent.LocalLobbyRefresh 
                }
            };
            NetWorkManager.instance.SendTcp(syncPackage);
            Debug.Log("请求刷新");
        }

        public void UI_JoinRoom()
        {
            //TODO:加入房间
        }

        public void UI_CreateRoom()
        {
            //TODO:创建房间
        }
        
        
        public void UI_OnBackButtonPressDown()
        {
            Signals.Get<BackToMainPressDownSignal>().Dispatch();
        }

        void Start()
        {
            NetWorkManager.instance.RegisterEventHandler<RefreshListResponsePackage>(NetEvent.LobbyRefresh,RefreshList);
        }
    }
}