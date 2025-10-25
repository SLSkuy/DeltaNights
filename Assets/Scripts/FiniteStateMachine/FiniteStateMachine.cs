using System;
using System.Collections.Generic;
using UnityEngine;

namespace FiniteStateMachine
{
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