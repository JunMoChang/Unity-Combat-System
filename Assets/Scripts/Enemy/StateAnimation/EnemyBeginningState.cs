using Player.StateManager.AbstractState;
using UnityEngine;

namespace Enemy.StateAnimation
{
    public class EnemyBeginningState : StateBase
    {
        private float currentSpeed;
        private const float Idle = 0f;
        private const float Run = 5f;
        
        private readonly EnemyFsm enemyFsm;
        public EnemyBeginningState(StateMachine stateMachine, Animator animator, int conditionHash) : base(stateMachine, animator, conditionHash)
        {
            stateName = nameof(EnemyBeginningState);
            enemyFsm = stateMachine as EnemyFsm;
        }
        
        public override void Enter()
        {
            animator.Play("BeginningState");
            UpdateSpeed();
        }
        
        public override void Update()
        {
            UpdateSpeed();
        }

        private void UpdateSpeed()
        {
            float targetSpeed = enemyFsm.enemyController.isMoving ? Run : Idle;
            currentSpeed = Mathf.Abs(currentSpeed - targetSpeed) > 0.05f ? Mathf.Lerp(currentSpeed, targetSpeed, enemyFsm.enemyController.step) : targetSpeed;
            animator.SetFloat(conditionHash, currentSpeed);
        }
        public override void Exit()
        {
            currentSpeed = 0f;
            animator.SetFloat(conditionHash, currentSpeed);
        }
    }
}