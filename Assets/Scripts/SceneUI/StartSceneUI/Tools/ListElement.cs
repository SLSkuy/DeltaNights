using UnityEngine;

namespace SceneUI.StartSceneUI
{
    public class ListElement:MonoBehaviour
    {
        public int _index { get; protected set; }//外界所用的索引
        
        public void SetIndex(int index)//修改序号
        {
            _index = index;
        }
    }
}