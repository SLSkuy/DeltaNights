using UIFramework.Core;
using UnityEngine;

namespace UIFramework.Panel
{
    /// <summary>
    /// 面板（Panel）的Layer
    /// 通过访问管理对象的controller进行面板的控制
    /// 面板是一种没有历史记录、队列关系的UI界面
    /// 通过优先级划分多个子面板层，模块化管理不同优先级的面板层
    /// </summary>
    public class PanelLayer : UIBaseLayer<IPanelController>
    {
        #region 内部成员

        /// <summary>
        /// 记录所有优先级子层
        /// </summary>
        [SerializeField] [Tooltip("优先级子层设置，注册到此层的面板将根据优先级分配到不同子层")]
        private PanelPriorityLayerList prioritySubLayers;

        #endregion
        
        #region 面板控制器管理方法

        public override void ShowUI(IPanelController controller) => controller.Show();
        public override void ShowUI<TProps>(IPanelController controller, TProps props) => controller.Show(props);
        public override void HideUI(IPanelController controller) => controller.Hide();
        
        #endregion
        
        #region 面板层管理方法

        /// <summary>
        /// 根据ID判断面板是否可见
        /// </summary>
        /// <param name="id">面板ID</param>
        public bool IsPanelVisible(string id)
        {
            return RegisteredUIControllers.TryGetValue(id, out var controller) && controller.IsVisible;
        }
        
        public override void ReParentUI(IUIController controller, Transform uiTransform)
        {
            // 检测是否为面板控制器
            if (controller is IPanelController panelController)
            {
                // 若是则根据面板优先级分配到对应的Layer层
                ReParentToParaLayer(panelController.Priority, uiTransform);
                return;
            }
            
            // 未知类型处理
            base.ReParentUI(controller, uiTransform);
            Debug.Log($"[PanelLayer] Unknown controller type {controller.GetType()}");
        }

        /// <summary>
        /// 根据优先级将不同的面板分到对应优先级的子Layer中
        /// </summary>
        /// <param name="priority">面板优先级</param>
        /// <param name="uiTransform">面板Transform</param>
        private void ReParentToParaLayer(PanelPriority priority, Transform uiTransform)
        {
            // 找到了对应优先级子层
            if (prioritySubLayers.LookupList.TryGetValue(priority, out var layerRootTransform))
            {
                uiTransform.SetParent(layerRootTransform,false);
                return;
            }
            
            // 未找到对应优先级子层
            uiTransform.SetParent(this.transform, false);
        }

        #endregion
    }
}