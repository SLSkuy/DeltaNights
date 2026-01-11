using System;
using EventProcess;
using LobbySyncPackage;
using Network;
using SyncPackage;
using TMPro;
using UIFramework.Window;
using UnityEngine;

namespace SceneUI.StartSceneUI
{
    public class CreateRoomWindow:WindowController
    {
        [SerializeField] private TMP_InputField inputName;
        [SerializeField] private TMP_InputField inputType;
        [SerializeField] private TMP_InputField inputIntroduction;

        void OnEnable()
        {   //清除残留输入
            inputName.text = string.Empty;
            inputType.text = string.Empty;
            inputIntroduction.text = string.Empty;
        }

        public void UI_OnCreateButtonPressDown()//创建房间请求
        {
            if (inputName.text.Length == 0)
            {
                Signals.Get<ErrorPromptWindowSignal>().Dispatch("房间名不能为空");
                return;
            }

            if (inputType.text.Length == 0)
            {
                Signals.Get<ErrorPromptWindowSignal>().Dispatch("类型不能为空");
                return;
            }

            if (inputIntroduction.text.Length == 0)
            {
                Signals.Get<ErrorPromptWindowSignal>().Dispatch("介绍不能为空");
                return;
            }
            //TODO:对接 发送创建房间请求
            LocalSyncPackage syncPackage = new LocalSyncPackage
            {
                EventID = LocalSyncEvent.LobbyRequest,
                LobbySync = new LobbySyncRequest
                {
                    EventID = LocalLobbyEvent.LocalLobbyRoomCreate,
                    RoomCreate = new RoomCreateRequest
                    {
                        RoomName = inputName.text,
                        RoomType = inputType.text,
                        RoomIntroduction = inputIntroduction.text
                    }
                }
            };
            Signals.Get<StatusPromptWindowSignal>().Dispatch(true,"创建房间中");
            NetWorkManager.instance.SendTcp(syncPackage);
            
        }
    }
}