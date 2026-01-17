using UnityEngine;

namespace Player.StateManager.AbstractState
{
    public abstract class StateBase
    {
        public string stateName;
        protected Animator animator;
        protected int conditionHash;
        protected StateMachine stateMachine;
        protected StateBase(StateMachine stateMachine, Animator animator, int conditionHash)
        {
            this.stateMachine = stateMachine;
            this.animator = animator;
            this.conditionHash = conditionHash;
        }
        public virtual void Enter(){}
        public virtual void Update(){}
        public virtual void Exit(){}

        protected AnimatorStateInfo GetCurrentStateInfo(int layerIndex)
        {
            return animator.GetCurrentAnimatorStateInfo(layerIndex);
        }
    }
}


