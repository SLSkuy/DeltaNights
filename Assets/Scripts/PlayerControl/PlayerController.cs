using PlayerControl.PlayerFSM;
using UnityEngine;

namespace PlayerControl
{
    public class PlayerController : MonoBehaviour
    {
        #region 内部成员
        
        private PlayerFiniteStateMachine _finiteStateMachine;

        #endregion
        
        private void Awake()
        {
            _finiteStateMachine = new PlayerFiniteStateMachine();
            _finiteStateMachine.SwitchState(PlayerState.Idle);
        }
        
        private void Update() => _finiteStateMachine.Update();
        private void FixedUpdate() => _finiteStateMachine.FixedUpdate();
        private void LateUpdate() => _finiteStateMachine.LateUpdate();
        private void OnAnimatorMove() => _finiteStateMachine.OnAnimatorMove();
    }
}