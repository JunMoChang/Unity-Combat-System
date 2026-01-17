using UnityEngine;
using WeaponConfiguration.AbstractWeapon;

namespace WeaponConfiguration.WeaponComponent
{
    public class SheathComponent : IWeaponComponent
    {
        private readonly Transform hiltGripPoint;
        private readonly Transform scabbardPos;
        
        public SheathComponent(Transform hiltGripPoint, Transform scabbardPos)
        {
            this.hiltGripPoint = hiltGripPoint;
            this.scabbardPos = scabbardPos;
        }
        
        public void Draw(Transform handWeaponPos)
        {
            hiltGripPoint.SetParent(handWeaponPos, true);
            hiltGripPoint.localPosition = Vector3.zero;
            hiltGripPoint.rotation = Quaternion.LookRotation(handWeaponPos.forward, Vector3.up);
        }
    
        public void Sheathe()
        {
            hiltGripPoint.SetParent(scabbardPos, true);
            hiltGripPoint.localPosition = Vector3.zero;
            hiltGripPoint.rotation = Quaternion.LookRotation(scabbardPos.forward, Vector3.up);
        }
        
        public void Update()
        {
            throw new System.NotImplementedException();
        }
    }
}