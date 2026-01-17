using System.Collections.Generic;

namespace BT
{
    public class Sequence : Node
    {
        public Sequence(List<(Node, System.Func<bool>)> children) : base(children){ }
        
        public override NodeState Evaluate()
        {
            foreach ((Node node, System.Func<bool> condition) child in children)
            {
                if(child.condition != null && child.condition())
                {
                    state = NodeState.Failure;
                    return state;
                }
                
                NodeState result = child.node.Evaluate();
                if (result != NodeState.Success)
                {
                    state = result;
                    return result;
                }
            }
            
            state = NodeState.Success;
            return state;
        }
    }
}