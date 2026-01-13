/* ------------------------------------------------------------
 *  Author:  2023051604032 WangXinKai
 *  Date:  2026.1.7
 *  LastUpdate:  2026.1.7
 * 
 *  功能简述：
 *  列表元素管理器 维护列表显示
 *  目前仅提供按序号操作
 *  待扩展索引方法
 * ------------------------------------------------------------ */

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SceneUI.StartSceneUI
{
    public class ListManager:MonoBehaviour
    {
        [SerializeField] protected GameObject _preObject;//生成的预制体
        
        public int _currentRoomIndex{get;private set;}//外界只读 当前的焦点序号
        
        
        protected Dictionary<int, GameObject> _elementByIndex=new Dictionary<int, GameObject>();
        // //双向字典
        // protected Dictionary<GameObject,int> _indexByElement=new Dictionary<GameObject,int>();
        
        // //双向字典 通过TKey
        // protected Dictionary<TKey, GameObject> _elementByKey=new Dictionary<TKey, GameObject>();
        // protected Dictionary<GameObject,TKey> _keyByElement=new Dictionary<GameObject,TKey>();

        public virtual void DeleteList()
        {
            foreach (var element in _elementByIndex.Values)
            {
                Destroy(element);
            }
            _elementByIndex.Clear();
        }

        //获取指定序号的元素
        public virtual GameObject GetElementByIndex(int index)
        {
            if (_elementByIndex.ContainsKey(index))
            {
                return _elementByIndex[index];
            }
            else
            {
                return null;
            }
        }
        
        //添加对应的元素 
        protected virtual GameObject AddElement(int index)
        {
            GameObject o=Instantiate(_preObject,this.transform);
            o.GetComponent<ListElement>().SetIndex(index);
            Button b=o.GetComponent<Button>();
            
            b.onClick.AddListener(() =>
            {
                OnElementClick(index);
            });
            
            _elementByIndex.Add(index, o);
            
            return o;
        }
        
        protected virtual void OnElementClick(int index)
        {
            Debug.Log("点击"+index);
            _currentRoomIndex=index;
        }
    }
}