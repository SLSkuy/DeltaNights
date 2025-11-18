using FiniteStateMachine;
using PlayerControl.PlayerFSM.PlayerStates;

namespace PlayerControl.PlayerFSM
{
    /// <summary>
    /// 玩家状态机
    /// </summary>
    public class PlayerFiniteStateMachine : FiniteStateMachine<PlayerState>
    {
        public PlayerFiniteStateMachine() : base()
        {
            AddState(PlayerState.Idle, new PlayerIdleState());
        }
    }
}
