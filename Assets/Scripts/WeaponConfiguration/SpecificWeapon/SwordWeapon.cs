using UnityEngine;
using WeaponConfiguration.AbstractWeapon;
using WeaponConfiguration.WeaponComponent;

namespace WeaponConfiguration.SpecificWeapon
{
    public class SwordWeapon : BaseWeapon
    {
        [SerializeField] private Transform hiltGripPoint;
        [SerializeField] private GameObject damageWeaponModel;
        [SerializeField] private Transform backToScabbardPos;
        
        private SheathComponent sheathComponent;
        private DamageComponent damageComponent;
        
        private void Start()
        {
            sheathComponent = new SheathComponent(hiltGripPoint, backToScabbardPos);
            damageComponent = new DamageComponent(damageWeaponModel);
            
            sheathComponent.Sheathe();
            AddWeaponComponent(sheathComponent);
            AddWeaponComponent(damageComponent);
        }
        
        
    }
}