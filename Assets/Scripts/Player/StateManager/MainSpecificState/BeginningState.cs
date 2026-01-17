using Player.StateManager.AbstractState;
using UnityEngine;

namespace Player.StateManager.MainSpecificState
{
    public class BeginningState : StateBase
    {
        private float currentSpeed;
        private const float Idlespeed = 0f;
        private const float Walkspeed = 1f;
        private const float Runspeed = 3f;
        private const float Sprintspeed = 8f;
        private int withdrawingSwordLayer;
        private readonly PlayerFsm playerFsm;
        public BeginningState(StateMachine stateMachine,Animator animator, int conditionHash) : base(stateMachine, animator, conditionHash)
        {
            playerFsm = stateMachine as PlayerFsm;
        }

        public override void Enter()
        {
            animator.SetLayerWeight(withdrawingSwordLayer, 0);
            playerFsm.isMoving = true;
            UpdateSpeed();
        }
        
        public override void Update()
        {
            if (playerFsm.isCombat)
            {
                stateMachine.ChangeState(playerFsm.combatState);
                return;
            }
            
            UpdateSpeed();
        }

        private void UpdateSpeed()
        {
            float targetSpeed = 0f;
            if(PlayerController.Instance.MoveInput != Vector2.zero)
            {
                if (InputManager.Instance.GetButton("Sprint"))
                {
                    targetSpeed = Sprintspeed;
                }
                else if (InputManager.Instance.GetButton("Walk"))
                {
                    targetSpeed = Walkspeed;
                }
                else
                {
                    targetSpeed = Runspeed;
                }
            }
            else
            { 
                targetSpeed = Idlespeed;
            }
            
            currentSpeed = Mathf.Abs(currentSpeed - targetSpeed) > 0.01f ? Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * Sprintspeed) : targetSpeed;
            animator.SetFloat(conditionHash, currentSpeed);
        }
        public override void Exit()
        {
            currentSpeed = 0f;
        }
    }
}