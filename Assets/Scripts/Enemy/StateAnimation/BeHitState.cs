using Player.StateManager.AbstractState;
using UnityEngine;

namespace Enemy.StateAnimation
{
    public class BeHitState : StateBase
    {
        private readonly EnemyFsm enemyFsm;
        private readonly int beHitLayer;
        private Coroutine checkCoroutine;
        public BeHitState(StateMachine stateMachine, Animator animator, int conditionHash) : base(stateMachine, animator, conditionHash)
        {
            stateName = nameof(BeHitState);
            enemyFsm = stateMachine as EnemyFsm;
            beHitLayer = animator.GetLayerIndex("BeHitLayer");
        }

        public override void Enter()
        {
            animator.SetLayerWeight(beHitLayer, 1);
            animator.Play("BeHitState", beHitLayer, 0f);
            
            if(checkCoroutine == null)
            {
                checkCoroutine = enemyFsm.enemyController.StartCoroutine(CheckAnimationEnd());
            }
        }

        public override void Update()
        {
        }

        public override void Exit()
        {
            animator.SetLayerWeight(beHitLayer, 0);
            enemyFsm.enemyController.isHit =  false; 
            
        }
        
        private System.Collections.IEnumerator CheckAnimationEnd()
        {
            yield return null;
        
            while(true)
            {
                AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(beHitLayer);
                
                if(animator.IsInTransition(beHitLayer))
                {
                    yield return null;
                    continue;
                }
                
                if (info.normalizedTime >= 1f)
                {
                    enemyFsm.ChangeState(enemyFsm.GetState(nameof(EnemyBeginningState)));
                    checkCoroutine = null;
                    yield break;
                }
                
                yield return null;
            }
        }
    }
}