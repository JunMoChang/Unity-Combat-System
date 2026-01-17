using System.Collections.Generic;

namespace Player.StateManager.AbstractState
{
    public abstract class StateMachine
    {
        public StateBase CurrentState { get; protected set; }
        private Dictionary<string, StateBase> stateDic = new ();
        
        public abstract void Start();
        public abstract void ChangeState(StateBase newState);
        
        protected virtual void InitializeStatesDic(List<StateBase> stateList)
        {
            foreach (StateBase state in stateList)
            {
                stateDic.TryAdd(state.stateName, state);
            }
        }
        
        public StateBase GetState(string stateName)
        {
            return stateDic.GetValueOrDefault(stateName);
        }
    }
}