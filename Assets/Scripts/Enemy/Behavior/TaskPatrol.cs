using BT;
using Player.StateManager.AbstractState;
using UnityEngine;

namespace Enemy.Behavior
{
    public class TaskPatrol : Node
    {
        private readonly Transform transform;
        private readonly Transform[] wayPoints;
        
        private int currentWaypointIndex;
        
        private readonly float waitTime = 1f;
        private float waitTimer;
        private bool waiting;

        private float currentSpeed;
        
        private readonly StateMachine enemyFsm;
        private StateBase currentState;
        private readonly EnemyController enemyController;
        public TaskPatrol(Transform transform, Transform[] wayPoints, StateMachine enemyFsm, EnemyController enemyController)
        {
            this.transform = transform;
            this.wayPoints = wayPoints;
            this.enemyFsm = enemyFsm;
            this.enemyController = enemyController;
        }
        public override NodeState Evaluate()
        {
            StateBase targetState = enemyFsm.GetState(nameof(StateAnimation.EnemyBeginningState));
            if(enemyFsm.CurrentState != targetState) enemyFsm.ChangeState(targetState);
            
            if (waiting)
            {
                enemyController.isMoving = false;
                waitTimer += Time.deltaTime;
                if(waitTimer >= waitTime) waiting = false;
            }
            else
            {
                Transform target = wayPoints[currentWaypointIndex];
                if (Vector3.Distance(transform.position, target.position) < 0.01f)
                {
                    transform.position = new Vector3(target.position.x, transform.position.y, target.position.z);
                    waitTimer = 0f;
                    waiting = true;
                    
                    currentWaypointIndex = (currentWaypointIndex +1 ) % wayPoints.Length;
                }
                else
                {
                    enemyController.isMoving = true;
                    enemyController. MoveToTarget(target);
                }
            }

            state = NodeState.Running;
            return state;
        }
        
    }
}