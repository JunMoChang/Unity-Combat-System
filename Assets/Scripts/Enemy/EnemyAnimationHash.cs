using System;
using UnityEngine;
namespace Enemy
{
    public class EnemyAnimationHash
    {
        public enum AnimationParameterType
        {
            EnemySpeed,
            Attacking,
            BeAttacked,
        }
        
        private static readonly int[] ParameterHashes;
        
        static EnemyAnimationHash()
        {
            Array parameters = Enum.GetValues(typeof(AnimationParameterType));
            ParameterHashes = new int[parameters.Length];
            
            foreach (AnimationParameterType param in parameters)
            {
                ParameterHashes[(int)param] = Animator.StringToHash(param.ToString());
            }
        }
        
        public static int GetParameterHash(AnimationParameterType parameter)
        {
            return ParameterHashes[(int)parameter];
        }
    }
}