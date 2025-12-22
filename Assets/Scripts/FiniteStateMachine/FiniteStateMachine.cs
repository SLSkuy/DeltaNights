/* ------------------------------------------------------------
 *  Author:  2023051604044 wanrui
 *  Date:  2025.10.25
 *  LastUpdate: 2025.11.19
 * 
 *  功能简述：
 *  FiniteStateMachine 为通用有限状态机实现，
 *  用于管理对象在不同状态之间的切换与行为更新。
 *
 *  主要功能：
 *  - 基于枚举类型管理状态映射关系
 *  - 统一处理状态进入、退出与切换流程
 *  - 向外提供标准化的更新接口
 *
 *  使用说明：
 *  - 状态需实现 IState 接口
 *  - 通过枚举类型作为状态标识添加到状态机
 *  - 在宿主对象的生命周期中调用对应 Update 方法
 * ------------------------------------------------------------ */

using System;
using System.Collections.Generic;
using UnityEngine;

namespace FiniteStateMachine
{
    /// <summary>
    /// 有限状态机
    /// </summary>
    /// <typeparam name="T">对象状态枚举类型</typeparam>
    public class FiniteStateMachine<T> where T : Enum
    {
        #region 内部成员

        private readonly Dictionary<T, IState> _states;
        private IState _currentState;

        #endregion
        
        #region 状态机方法

        protected FiniteStateMachine()
        {
            _states = new Dictionary<T, IState>();
        }

        /// <summary>
        /// 添加状态到状态机中
        /// </summary>
        /// <param name="stateType">枚举状态标识</param>
        /// <param name="state">实现 IState 接口的状态示例</param>
        public void AddState(T stateType, IState state)
        {
            if (!_states.TryAdd(stateType, state))
            {
                Debug.Log("[FSM] map has contain key: " + stateType);
            }
        }
        
        /// <summary>
        /// 删除状态机中的状态
        /// </summary>
        /// <param name="stateType">枚举状态标识</param>
        public void RemoveState(T stateType)
        {
            _states.Remove(stateType);
        }

        /// <summary>
        /// 切换状态机的当前状态
        /// </summary>
        /// <param name="stateType">枚举状态标识</param>
        public void SwitchState(T stateType)
        {
            if (!_states.TryGetValue(stateType, out IState state))
            {
                Debug.Log("[switchState] >>>>>>>>>>> not contain key: " + stateType);
                return;
            }

            _currentState?.Exit();
            _currentState = state;
            _currentState.Enter();
        }
        
        #endregion

        #region 更新方法
        
        public void Update() => _currentState.Update();
        public void FixedUpdate() => _currentState.FixedUpdate();
        public void LateUpdate() => _currentState.LateUpdate();
        public void OnAnimatorMove() => _currentState.OnAnimatorMove();

        #endregion
    }
}