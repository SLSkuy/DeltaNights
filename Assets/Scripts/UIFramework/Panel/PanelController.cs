using UIFramework.Core;

namespace UIFramework.Panel
{
    /// <summary>
    /// 面板控制器基类
    /// </summary>
    public class PanelController : UIController<PanelProperties>, IPanelController
    {
        #region 暴露属性

        public PanelPriority Priority => Properties?.Priority ?? PanelPriority.None;

        #endregion
        
        #region 控制器方法

        protected sealed override void SetProperties(PanelProperties properties)
        {
            base.SetProperties(properties);
        }
        
        #endregion
    }
}