using Player.StateManager.AbstractState;
using UnityEngine;

namespace Enemy.StateAnimation
{
    public class EnemyCombatState : StateBase
    {
        
        public EnemyCombatState(StateMachine stateMachine, Animator animator, int conditionHash) : base(stateMachine, animator, conditionHash)
        {
            stateName = nameof(EnemyCombatState);
        }

        public override void Enter()
        {
            base.Enter();
        }

        public override void Update()
        {
            base.Update();
        }

        public override void Exit()
        {
            base.Exit();
        }
    }
}