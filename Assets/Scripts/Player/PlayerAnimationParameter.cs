using System;
using UnityEngine;

namespace Player
{
    public class PlayerAnimationParameter
    {
        public enum AnimationParameterType
        {
            Speed,
            Combat,
            HasWeapon,
            Attacking,
            Combos,
            HasWithdrawingSword
        }
        private int[] parametersHash;
        
        public PlayerAnimationParameter()
        {
            Initialize();
        }

        private void Initialize()
        {
            Array parameters = Enum.GetValues(typeof(AnimationParameterType));
            parametersHash = new int[parameters.Length];

            foreach (AnimationParameterType param in parameters)
            {
                parametersHash[(int)param] = Animator.StringToHash(param.ToString());
            }
        }
            
        public int GetParameterHash(AnimationParameterType parameterType)
        {
            return parametersHash[(int)parameterType];
        }
    }
}