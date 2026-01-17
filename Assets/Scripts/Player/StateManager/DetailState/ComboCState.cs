using Player.StateManager.AbstractState;
using UnityEngine;

namespace Player.StateManager.DetailState
{
    public class ComboCState : StateBase
    {
        private int combos;
        private readonly float combosCondition = 0.85f;
        private bool isAttacking;
        
        private readonly PlayerFsm playerFsm;
        public ComboCState(StateMachine stateMachine, Animator animator, int conditionHash) : base(stateMachine, animator, conditionHash)
        {
            playerFsm = stateMachine as PlayerFsm;
        }

        public override void Enter()
        {
            animator.SetBool(PlayerController.Instance.animationParameter.GetParameterHash(PlayerAnimationParameter.AnimationParameterType.Attacking), true);
            
            animator.SetInteger(conditionHash, ++combos);
            playerFsm.isMoving = false;
            PlayerController.Instance.attackAction += Combos;
        }

        public override void Update()
        {
            AnimatorStateInfo stateInfo = GetCurrentStateInfo(0);
            
            if (!stateInfo.IsTag("Attack") || animator.IsInTransition(0)) return;
           
            float process = stateInfo.normalizedTime;
            
            if (process < 0.4f) PlayerController.Instance.attackSetting.LockRotationToNearestEnemy();

            switch (process)
            {
                case <= 1 when process >= combosCondition && isAttacking:
                    animator.SetInteger(conditionHash, ++combos);
                    playerFsm.isMoving = false;
                    isAttacking = false;
                    break;
                case > 1:
                    playerFsm.ChangeState(playerFsm.combatState);
                    break;
            }
        }

        public override void Exit()
        {
            playerFsm.isMoving = true;
            combos = 0;
            isAttacking = false;
            animator.SetInteger(conditionHash, 0);
            animator.SetBool(PlayerController.Instance.animationParameter.GetParameterHash(PlayerAnimationParameter.AnimationParameterType.Attacking), isAttacking);
            PlayerController.Instance.attackAction -= Combos;
        }
        
        private void Combos(string actionName, UnityEngine.InputSystem.InputControl inputControl)
        {
            if (playerFsm.CurrentState != playerFsm.comboCState) return;
            
            isAttacking = true;
            
        }
    }
}