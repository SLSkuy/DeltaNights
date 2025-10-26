using UIFramework.Core;
using UnityEngine.UIElements;

namespace UIFramework.Panel
{
    /// <summary>
    /// 面板控制器基类，控制面板的各种行为
    /// </summary>
    public class PanelController : UIController<IPanelProperties>, IPanelController
    {
        #region 暴露属性

        // 若属性为空则设置默认属性
        public PanelPriority Priority => Properties?.Priority ?? PanelPriority.None;

        #endregion
        
        // #region 控制器方法
        //
        // protected sealed override void SetProperties(PanelProperties props)
        // {
        //     base.SetProperties(props);
        // }
        //
        // #endregion
    }
}