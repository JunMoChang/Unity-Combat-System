using BT;
using Player.StateManager.AbstractState;
using UnityEngine;

namespace Enemy.Behavior
{
    public class TaskGoToTarget : Node
    {
        private readonly Transform transform;
        private readonly StateMachine enemyFsm;
        private readonly EnemyController enemyController;
        public TaskGoToTarget(Transform trans, StateMachine enemyFsm, EnemyController enemyController)
        {
            transform = trans;
            this.enemyFsm = enemyFsm;
            this.enemyController = enemyController;
        }

        public override NodeState Evaluate()
        {
            Transform target = (Transform)GetData("target");
            
            if (Vector3.Distance(transform.position, target.position) > 1f)
            {
                StateBase targetState = enemyFsm.GetState(nameof(StateAnimation.EnemyBeginningState));
                if (enemyFsm.CurrentState != targetState) enemyFsm.ChangeState(targetState);
                enemyController.isMoving = true;
                enemyController.MoveToTarget(target);
            }
            else
            {
                enemyController.isMoving = false;
            }

            state = NodeState.Running;
            return state;
        }
        
    }
}