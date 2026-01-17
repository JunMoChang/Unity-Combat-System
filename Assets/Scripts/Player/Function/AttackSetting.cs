using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using WeaponConfiguration.SpecificWeapon;

namespace Player.Function
{
    public enum AttackType
    {
        Normal,
        Heavy
    }
    [Serializable]
    public class AttackSetting
    {
        [Header("输入检测")]
        [SerializeField] private int maxInputTimes = 5;
        [SerializeField] private float maxInputInterval;
        private int currentInputTimes;
        private float inputResetTimes;
        
        private readonly Transform playerTransform;
        private readonly LayerMask enemyLayer;
        private readonly Collider[] enemies = new Collider[3];
        public readonly Dictionary<AttackType, string>  attackMapping = new ()
        {
            { AttackType.Normal, "/Mouse/leftButton" },
            { AttackType.Heavy, "/Mouse/rightButton" },
        };

        private WeaponHandel weaponHandel;
        private WeaponDamage currentWeaponDamage;
        public AttackSetting(Transform playerTransform, WeaponHandel weaponHandel, LayerMask enemyLayer)
        {
            this.playerTransform = playerTransform;
            this.weaponHandel = weaponHandel;
            this.enemyLayer = enemyLayer;
        }
        public void InputKeySetting(string actionName, InputControl inputControl, Action<string, InputControl> attackAction)
        {
            if(actionName != "Attack") return;
            
            PlayerController.Instance.playerFsm.isCombat = true;
            
            if (CheckInputFrequency())
            {
                attackAction?.Invoke(actionName, inputControl);
            }
        }
        
        private bool CheckInputFrequency() 
        {
            if (Time.time >= inputResetTimes)
            {
                currentInputTimes = 1;
                inputResetTimes = Time.time + maxInputInterval;    
            }
            
            if (currentInputTimes >= maxInputTimes)
            {
                return false;
            }
            
            currentInputTimes++;
            return true;
        }

        public void LockRotationToNearestEnemy()
        {
            int num = Physics.OverlapSphereNonAlloc(playerTransform.position, 10f, enemies,enemyLayer);
            if (num > 0)
            {
                Vector3 direction = (enemies[0].transform.position - playerTransform.position).normalized;
                playerTransform.rotation = Quaternion.LookRotation(direction);
            }
        }
        public void OnEnableAttack()
        {
            if(currentWeaponDamage == null) currentWeaponDamage = weaponHandel.damageModel.GetComponent<WeaponDamage>();
            
            currentWeaponDamage.ResetDamage();
        }
        
    }
    
}