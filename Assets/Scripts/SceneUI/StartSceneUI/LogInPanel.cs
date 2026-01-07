/* ------------------------------------------------------------
 *  Author:  2023051604032 WangXinKai
 *  Date:  2026.1.2
 *  LastUpdate:  2026.1.2
 * 
 *  功能简述：
 *  登录界面 目前仅有用户名 + 密码
 *  
 * ------------------------------------------------------------ */

using ClientSyncPackage;
using EventProcess;
using Network;
using SyncPackage;
using TMPro;
using UIFramework.Panel;
using UnityEngine;


namespace SceneUI.StartSceneUI
{
    public class LogInPressDownSignal:ASignal<string>{}
    
    public class LogInPanel:PanelController
    {
        [SerializeField] private TMP_InputField inputAccount;
        [SerializeField] private  TMP_InputField inputPassword; 
        public void UI_OnLogInButtonPressDown()
        {
            if (inputAccount.text.Length > 0)
            {
                if (inputPassword.text.Length > 0)
                {
                    LocalSyncPackage syncPackage = new LocalSyncPackage
                    {
                        EventID = LocalSyncEvent.ClientRequest,
                        ClientSync = new ClientSyncRequest
                        {
                            EventID = LocalClientEvent.LoginRequest,
                            LoginRequest = new LoginRequestPackage
                            {
                                Account = inputAccount.text,
                                Password = inputPassword.text
                            }
                        }
                    };
                    NetWorkManager.instance.SendTcp(syncPackage);
                    Signals.Get<StatusPromptWindowSignal>().Dispatch(true,"登录中");
                }
                else Signals.Get<ErrorPromptWindowSignal>().Dispatch("密码不能为空");
            }
            else
            {
                Signals.Get<ErrorPromptWindowSignal>().Dispatch("用户名不能为空");
            }
        }

        //处理服务器的包
        void Login(LoginResponsePackage response)
        {
            Debug.Log(response.Uuid + "-" + response.NickName);
            Signals.Get<LogInPressDownSignal>().Dispatch(response.NickName);
        }

        public void UI_OnTestEnter()
        {
            Signals.Get<LogInPressDownSignal>().Dispatch("测试模式");
        }

        void Start()
        {
            NetWorkManager.instance.RegisterEventHandler<LoginResponsePackage>(NetEvent.LoginResponse,Login);    
        }

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
        }
    }
}