using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace InputProcess
{
    public class GameInput : MonoBehaviour
    {
        private InputSystem_Actions InputActions { get; set; }
        public InputSystem_Actions.PlayerActions PlayerActions { get; private set; }

        void Awake()
        {
            InputActions = new InputSystem_Actions();
            PlayerActions = new InputSystem_Actions.PlayerActions();
        }
        
        private void OnEnable()
        {
            InputActions.Enable();
        }

        private void OnDisable()
        {
            InputActions.Disable();
        }

        /// <summary>
        /// 禁用某一输入一段时间
        /// </summary>
        /// <param name="action">操作类</param>
        /// <param name="sec">时间</param>
        public void DisableActionForSec(InputAction action, float sec)
        {
            StartCoroutine(DisableAction(action, sec));
        }

        private IEnumerator DisableAction(InputAction action, float sec)
        {
            action.Disable();
            yield return new WaitForSeconds(sec);
            action.Enable();
        }
    }
}
