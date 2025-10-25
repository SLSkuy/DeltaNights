using FiniteStateMachine;
using PlayerControl.PlayerFSM.PlayerStates;

namespace PlayerControl.PlayerFSM
{
    public class PlayerFiniteStateMachine : FiniteStateMachine<PlayerState>
    {
        public PlayerFiniteStateMachine() : base()
        {
            AddState(PlayerState.Idle, new PlayerIdleState());
        }
    }
}
