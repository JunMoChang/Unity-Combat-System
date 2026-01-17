using System.Collections.Generic;

namespace BT
{
    public enum NodeState
    {
        Running,
        Success,
        Failure
    }

    public class Node
    {
        protected readonly List<(Node node, System.Func<bool> condition)> children = new();
        private readonly Dictionary<string, object> dataContext = new();

        public Node parent;
        protected NodeState state;
        
        protected Node()
        {
        }
        
        
        protected Node(List<(Node node, System.Func<bool> condition)> children)
        {
            foreach ((Node, System.Func<bool>) child in children)
            {
                Attach(child);
            }   
        }
        
        private void Attach((Node node, System.Func<bool> condition) child)
        {
            child.node.parent = this;
            children.Add(child);
        }
        
        public virtual NodeState Evaluate()
        {
            
            return NodeState.Failure;
        }
        
        public virtual void SetData(string key, object value)
        {
            dataContext[key] = value;
        }

        protected object GetData(string key)
        {
            if (dataContext.TryGetValue(key, out object value)) return value;

            Node node = parent;
            while (node != null)
            {
                value = node.GetData(key);
                if (value != null)
                    return value;
                node = node.parent;
            }

            return null;
        }

        public bool ClearData(string key)
        {
            if (dataContext.Remove(key))
            {
                return true;
            }

            var node = parent;
            while (node != null)
            {
                var cleared = node.ClearData(key);
                if (cleared)
                    return true;
                node = node.parent;
            }

            return false;
        }
    }
}