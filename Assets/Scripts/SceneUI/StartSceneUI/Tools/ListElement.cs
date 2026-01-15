/* ------------------------------------------------------------
 *  Author:  2023051604032 WangXinKai
 *  Date:  2026.1.7
 *  LastUpdate:  2026.1.7
 * 
 *  功能简述：
 *  列表内元素底层
 * 
 * ------------------------------------------------------------ */

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