using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace BT
{
    public abstract class BehaviorTree : MonoBehaviour
    {
        private Node root = null;
        private CancellationTokenSource evaluationCts = new ();
        protected virtual void Start()
        {
            root = SetUpTree();
            
        }
        
        protected virtual void Update()
        {
            if (root != null)
            {
                root.Evaluate();
            }
        }
        
        protected abstract Node SetUpTree();
    }
}