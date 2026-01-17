using System.Collections.Generic;
using Player.Function;
using Player.StateManager.AbstractState;
using Player.StateManager.DetailState;
using Player.StateManager.MainSpecificState;


namespace Player.StateManager
{
    public class PlayerFsm : StateMachine
    {
        public BeginningState BeginningState;
       
        public CombatState combatState;
        public ComboCState comboCState;
        
        public bool isCombat = false;
        public bool isMoving = false;
        
        public readonly Dictionary<string, StateBase> attackStates = new ();
        private readonly Dictionary<AttackType, string> attackMapping;

        public PlayerFsm(Dictionary<AttackType, string> attackMapping)
        {
            this.attackMapping = attackMapping;
        }
        
        public override void Start()
        {
            combatState =  new CombatState(this, PlayerController.Instance.animator, PlayerController.Instance.animationParameter.GetParameterHash(PlayerAnimationParameter.AnimationParameterType.Speed));
            BeginningState = new BeginningState(this, PlayerController.Instance.animator, PlayerController.Instance.animationParameter.GetParameterHash(PlayerAnimationParameter.AnimationParameterType.Speed));
            
            comboCState = new ComboCState(this, PlayerController.Instance.animator, PlayerController.Instance.animationParameter.GetParameterHash(PlayerAnimationParameter.AnimationParameterType.Combos));
            attackStates.Add(attackMapping[AttackType.Normal], comboCState);
            
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
            CurrentState = BeginningState;
            CurrentState.Enter();
        }
    }
}