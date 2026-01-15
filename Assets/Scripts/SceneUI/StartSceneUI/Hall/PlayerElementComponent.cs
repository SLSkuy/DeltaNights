using TMPro;
using UnityEngine;

namespace SceneUI.StartSceneUI
{
    public class PlayerElementComponent:ListElement
    {
        private string _name;
        
        [SerializeField] private TMP_Text _text;

        public void SetValue(string name)
        {
            _name = name;
            if (name != null)
            {
                _text.text = name;
            }
        }
    }
}