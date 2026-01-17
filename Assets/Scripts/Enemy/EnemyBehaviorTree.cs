using System;
using System.Collections.Generic;
using BT;
using Enemy.Behavior;
using Player.StateManager.AbstractState;
using UnityEngine;

namespace Enemy
{
    public class EnemyBehaviorTree : BehaviorTree
    {
        public Transform[] wayPoints;
        
        public LayerMask playerMask;
        
        private StateMachine enemyFsm;
        private EnemyController enemyController;
        protected override void Start()
        {
            enemyController = GetComponent<EnemyController>();
            enemyFsm = new EnemyFsm(this);
            enemyFsm.Start();
            base.Start();
        }

        protected override void Update()
        {
            base.Update();
            enemyFsm.CurrentState.Update();
        }
        protected override Node SetUpTree()
        {
            Node traceNode = new Sequence(new List<(Node, Func<bool>)>
            {
                (new CheckEnemyInFovRange(transform, playerMask, enemyController), () => enemyController.isHit),
                (new TaskGoToTarget(transform, enemyFsm, enemyController), () => enemyController.isHit)
            });

            Node patrolNode = new TaskPatrol(transform, wayPoints, enemyFsm, enemyController);
            
            Node root = new Selector(new List<(Node, Func<bool>)>
            {
                (new HitReactionNode(enemyController, enemyFsm), null),
                (traceNode, () => enemyController.isHit),
                (patrolNode, () => enemyController.isHit)
            });
            
            return root;
        }
    }
}