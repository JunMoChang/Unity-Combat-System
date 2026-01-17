using System.Collections.Generic;

namespace BT
{
    public class Selector : Node
    {
        public Selector(List<(Node node, System.Func<bool> condition)> children) : base(children){ }
        
        public override NodeState Evaluate()
        {
            foreach ((Node node, System.Func<bool> condition) child in children)
            {
                if(child.condition != null && child.condition()) continue;
                
                NodeState result = child.node.Evaluate();
                if (result != NodeState.Failure)
                {
                    state = result;
                    return result;
                }
            }
            
            state = NodeState.Failure;
            return state;
        }
    }
}