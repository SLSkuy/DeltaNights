/* ------------------------------------------------------------
 *  Author:  2023051604044 wanrui
 *  Date:  2025.10.25
 *  LastUpdate: 2025.11.19
 * 
 *  功能简述：
 *  IState 定义有限状态机中单个状态的行为接口规范。
 *
 *  使用说明：
 *  - 所有状态需实现该接口
 *  - Enter / Exit 用于状态切换时的初始化与清理
 *  - Update 系列方法由状态机在对应生命周期中调用
 * ------------------------------------------------------------ */

namespace FiniteStateMachine
{
    /// <summary>
    /// 状态机各状态接口
    /// </summary>
    public interface IState
    {
        void Enter();
        void Exit();
        void Update();
        void LateUpdate();
        void FixedUpdate();
        void OnAnimatorMove();
    }
}