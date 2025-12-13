using Network;
using UIFramework.Window;
using UnityEngine.UI;

namespace SceneUI.StartSceneUI
{
    public class ChatWindow : WindowController
    {
        public InputField inputField;
        public Text receiveText;
        
        void Start()
        {
            inputField.onSubmit.AddListener(OnSubmit);

            NetWorkManager.Instance.ReceiveMessage += ChangeText;
        }

        void OnSubmit(string text)
        {
            NetWorkManager.Instance.SendMsg(text);
        }

        void ChangeText(string text)
        {
            receiveText.text = text;
        }
        
        void OnDestroy()
        {
            inputField.onSubmit.RemoveListener(OnSubmit);
        }
    }
}