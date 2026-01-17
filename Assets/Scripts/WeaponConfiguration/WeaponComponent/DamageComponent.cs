using UnityEngine;
using WeaponConfiguration.AbstractWeapon;

namespace WeaponConfiguration.WeaponComponent
{
    public class DamageComponent : IWeaponComponent
    {
        private readonly GameObject damageModel;

        public DamageComponent(GameObject damageModel)
        {
            this.damageModel = damageModel;
        }

        public GameObject GetDamageModel()
        {
            return damageModel;
        }
        
        public void Update()
        {
            throw new System.NotImplementedException();
        }
    }
}