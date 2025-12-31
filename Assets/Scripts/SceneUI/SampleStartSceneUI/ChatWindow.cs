using Network;
using UIFramework.Window;
using UnityEngine.UI;

namespace SceneUI.SampleStartSceneUI
{
    public class ChatWindow : WindowController
    {
        public InputField inputField;
        public Text receiveText;
        
        void Start()
        {
            inputField.onSubmit.AddListener(OnSubmit);
        }

        void OnSubmit(string text)
        {
            
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