using Enemy;
using UnityEngine;

namespace WeaponConfiguration.SpecificWeapon
{
    public class WeaponDamage : MonoBehaviour
    {
        public float damage;
        private bool canDamage;
        void OnTriggerEnter(Collider collision)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                Debug.Log("敌人");
                if (!canDamage) return;
                
                if (collision.gameObject.TryGetComponent(out EnemyController enemy))
                {
                    canDamage = false;
                    enemy.TakeDamage(damage);
                }
            }
        }

        public void ResetDamage()
        {
            canDamage = true;
        }
    }
}
