using FiniteStateMachine;
using UnityEngine;

namespace PlayerControl.PlayerFSM.PlayerStates
{
    public class PlayerBaseState : IState
    {
        #region IState Members

        public virtual void Enter()
        {
            Debug.Log("Enter " + GetType().Name + " State");
        }

        public virtual void Exit()
        {

        }

        public virtual void Update()
        {

        }

        public virtual void FixedUpdate()
        {

        }

        public virtual void LateUpdate()
        {

        }

        public virtual void OnAnimatorMove()
        {

        }

        #endregion
    }
}