using System.Collections;
using Player.StateManager.AbstractState;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player.StateManager.MainSpecificState
{
    public class CombatState : StateBase
    {
        private const float IdleSpeed = 0f;
        private const float WalkSpeed = 1f;
        private const float RunSpeed = 3f;
        private const float SprintSpeed = 8f;

        private readonly PlayerFsm playerFsm;

        private readonly int withdrawingSwordLayer;
        private float currentSpeed;
        private bool hasWithdrawingSword;

        public CombatState(StateMachine stateMachine, Animator animator, int conditionHash) : base(stateMachine, animator, conditionHash)
        {
            playerFsm = stateMachine as PlayerFsm;
            withdrawingSwordLayer = animator.GetLayerIndex("WithdrawingSwordLayer");
            PlayerController.Instance.attackAction += Combos;
        }

        public override void Enter()
        {
            animator.SetBool(PlayerController.Instance.animationParameter.GetParameterHash(PlayerAnimationParameter.AnimationParameterType.Combat), true);
            if (!hasWithdrawingSword && PlayerController.Instance.CheckHasWeapon())
            {
                PlayerController.Instance.BeginCoroutine(WithdrawSwordLayerSetting(1, "WithdrawingSword"));
                hasWithdrawingSword =  true;
            }
            
            UpdateSpeed(); 
        }

        public override void Update()
        {
            if (!playerFsm.isCombat) playerFsm.ChangeState(playerFsm.BeginningState);
            UpdateSpeed();
        }

        public override void Exit()
        {
            if (!playerFsm.isCombat)
            {
                animator.SetLayerWeight(withdrawingSwordLayer, 0);
                if (PlayerController.Instance.CheckHasWeapon())
                {
                    PlayerController.Instance.BeginCoroutine(WithdrawSwordLayerSetting(1, "SheatheSword"));
                }
                
                hasWithdrawingSword = false;
                animator.SetBool(PlayerController.Instance.animationParameter.GetParameterHash(PlayerAnimationParameter.AnimationParameterType.Combat), false);
                animator.SetBool(PlayerController.Instance.animationParameter.GetParameterHash(PlayerAnimationParameter.AnimationParameterType.HasWithdrawingSword), false);
            }
        }

        private IEnumerator WithdrawSwordLayerSetting(int weight, string name)
        {
            animator.SetLayerWeight(withdrawingSwordLayer, weight);
            animator.Play(name, withdrawingSwordLayer, 0f);
            yield return null;
            
            while (true)
            {
                AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(withdrawingSwordLayer);
                if (stateInfo.normalizedTime >= 1.0f) break;
                
                yield return null;
            }
            
            animator.SetBool(PlayerController.Instance.animationParameter.GetParameterHash(PlayerAnimationParameter.AnimationParameterType.HasWithdrawingSword), hasWithdrawingSword);   
            animator.SetLayerWeight(withdrawingSwordLayer, 0);
        }

        private void Combos(string actionName, InputControl inputControl)
        {
            if(!PlayerController.Instance.CheckHasWeapon()) return;
            if (playerFsm.CurrentState != playerFsm.combatState) return;
            
            if (!hasWithdrawingSword)
            {
                Debug.Log("开启协程");
                PlayerController.Instance.BeginCoroutine(WithdrawSwordLayerSetting(1, "WithdrawingSword"));
                hasWithdrawingSword =  true;
            }
            
            if(animator.GetBool(PlayerController.Instance.animationParameter.GetParameterHash(PlayerAnimationParameter.AnimationParameterType.HasWithdrawingSword)))
            {
                if (playerFsm.attackStates.TryGetValue(inputControl.path, out StateBase state)) playerFsm.ChangeState(state);
            }
        }

        private void UpdateSpeed()
        {
            float targetSpeed;
            if (PlayerController.Instance.MoveInput != Vector2.zero)
            {
                if (InputManager.Instance.GetButton("Sprint"))
                    targetSpeed = SprintSpeed;
                else if (InputManager.Instance.GetButton("Walk"))
                    targetSpeed = WalkSpeed;
                else
                    targetSpeed = RunSpeed;
            }
            else
            {
                targetSpeed = IdleSpeed;
            }

            currentSpeed = Mathf.Abs(currentSpeed - targetSpeed) > 0.01f
                ? Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * SprintSpeed)
                : targetSpeed;
            animator.SetFloat(conditionHash, currentSpeed);
        }
    }
}