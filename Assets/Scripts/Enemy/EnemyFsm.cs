using System.Collections.Generic;
using Enemy.StateAnimation;
using Player.StateManager.AbstractState;
using UnityEngine;

namespace Enemy
{
    public class EnemyFsm : StateMachine
    {
        private List<StateBase> stateList;
        public readonly EnemyController enemyController;

        public EnemyFsm(MonoBehaviour mono)
        {
            enemyController = mono.GetComponent<EnemyController>();
        }
        
        public override void Start()
        {
            stateList = new List<StateBase>
            { 
                new EnemyBeginningState(this, enemyController.animator, EnemyAnimationHash.GetParameterHash(EnemyAnimationHash.AnimationParameterType.EnemySpeed)),
                new EnemyCombatState(this, enemyController.animator, EnemyAnimationHash.GetParameterHash(EnemyAnimationHash.AnimationParameterType.Attacking)),
                new BeHitState(this, enemyController.animator, EnemyAnimationHash.GetParameterHash(EnemyAnimationHash.AnimationParameterType.BeAttacked))
            };
            
            InitializeStatesDic(stateList);
            SetInitialState();
        }
        
        public override void ChangeState(StateBase newState)
        {
            if(newState == CurrentState) return;
            CurrentState.Exit();
            CurrentState = newState;
            if(newState != null)
            {
                CurrentState.Enter();
            }
        }
        private void SetInitialState()
        {
            CurrentState = stateList[0];
            CurrentState.Enter();
        }
    }
}