using System;
using System.Collections.Generic;
using UnityEngine;

namespace UIFramework.Window
{
    /// <summary>
    /// 带蒙黑的辅助层
    /// 用于显示弹窗等窗口（如二次确认提示框等）
    /// </summary>
    [Serializable]
    public class WindowPriorityLayer : MonoBehaviour
    {
        /// <summary>
        /// 蒙黑面板对象
        /// </summary>
        [SerializeField] private Transform darkenBgObject;
        
        /// <summary>
        /// 存储所有辅助界面
        /// </summary>
        private List<Transform> _containedUIs = new List<Transform>();

        public void AddUI(Transform ui)
        {
            ui.SetParent(transform, false);
            _containedUIs.Add(ui);
        }

        /// <summary>
        /// 辅助界面显示时，打开蒙黑面板限制只能与辅助界面交互
        /// </summary>
        public void RefreshDarken()
        {
            foreach (Transform ui in _containedUIs)
            {
                if (ui.gameObject.activeSelf)
                {
                    darkenBgObject.gameObject.SetActive(true);
                    return;
                }
            }
            
            darkenBgObject.gameObject.SetActive(false);
        }

        /// <summary>
        /// 让蒙黑面板显示在最上层以覆盖其他UI界面
        /// </summary>
        public void DarkenBg()
        {
            darkenBgObject.gameObject.SetActive(true);
            darkenBgObject.transform.SetAsLastSibling();
        }
    }
}