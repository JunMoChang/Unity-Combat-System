using BT;
using Player.StateManager.AbstractState;

namespace Enemy.Behavior
{
    public class HitReactionNode : Node
    {
        private readonly StateMachine enemyFsm;
        private readonly EnemyController enemyController;
        public HitReactionNode(EnemyController enemyController, StateMachine enemyFsm)
        {
            this.enemyController = enemyController;
            this.enemyFsm = enemyFsm;
        }
        
        public override NodeState Evaluate()
        {
            if (!enemyController.isHit)
            {
                state = NodeState.Failure;
                return state;
            }

            StateBase targetState = enemyFsm.GetState(nameof(StateAnimation.BeHitState));
            if(enemyFsm.CurrentState != targetState) enemyFsm.ChangeState(targetState);

            state = NodeState.Running;
            return state;
        }
    }
}