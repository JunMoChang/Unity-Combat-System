using BT;
using UnityEngine;

namespace Enemy.Behavior
{
    public class CheckEnemyInFovRange : Node
    {
        private readonly Transform transform;
        private const int MaxNumber = 3;
        private readonly Collider[] colliders =  new Collider[MaxNumber];
        private readonly LayerMask playerMask;
        private readonly EnemyController enemyController;
        public CheckEnemyInFovRange(Transform trans, LayerMask mask, EnemyController enemyController)
        {
            transform = trans;
            playerMask = mask;
            this.enemyController = enemyController;
        }

        public override NodeState Evaluate()
        {
            int length = Physics.OverlapSphereNonAlloc(transform.position, enemyController.sightRadius, colliders, playerMask);
            
            if (length > 0)
            {
                parent.parent.SetData("target", colliders[0].transform);
            }
            else
            {
                state = NodeState.Failure;
                return state;
            }
            
            state = NodeState.Success;
            return state;
        }
    }
}