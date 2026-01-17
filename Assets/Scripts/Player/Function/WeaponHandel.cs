using System;
using System.Collections.Generic;
using Scriptable;
using UnityEngine;
//using UnityEngine.Animations.Rigging;
using WeaponConfiguration.AbstractWeapon;
using WeaponConfiguration.WeaponComponent;

namespace Player.Function
{
    public class WeaponHandel : MonoBehaviour
    {
        [Header("射线设置")] 
        [SerializeField] [Range(0f, 3.5f)]
        private float headLength;
        [SerializeField] [Range(0f, 1f)] private float handLength;
        [SerializeField] private Vector3 rayOffset;
        [SerializeField] private LayerMask interactionMask;
        [SerializeField] private Transform headPosition;
        private readonly RaycastHit[] results =  new RaycastHit[3];
        private Ray faceRay;
        
        private bool isCatch;
        private RaycastHit lHandHitInfo;
        private Ray lHandRay;
        private RaycastHit rHandHitInfo;
        private Ray rHandRay;

        [Header("武器位置")] 
        [SerializeField] private Transform lWaistWeaponPos;
        [SerializeField] private Transform rWaistWeaponPos;
        [SerializeField] private Transform bWaistWeaponPos;
        [SerializeField] private Transform lHandWeaponPos;
        [SerializeField] private Transform rHandWeaponPos;
        private List<Transform> weaponPositions;
        private readonly Dictionary<Transform, BaseWeapon> weaponsDic = new ();
        //[Header("手部IK")] 
        //[SerializeField] private TwoBoneIKConstraint lHand;
        //SerializeField] private TwoBoneIKConstraint rHand;

        public bool HasWeapon { get; private set; }
        private BaseWeapon currentWeapon;
        [NonSerialized]public GameObject damageModel;
        
        public Func<ItemScriptableObject, int> inventoryAddWeaponAction;
        
        private void Start()
        {
            InputManager.Instance.OnInputPressed += OnInputPressed;
            
            weaponPositions = new List<Transform>
            {
                lWaistWeaponPos,
                bWaistWeaponPos,
                rWaistWeaponPos
            };
            
            faceRay = new Ray(headPosition.position + rayOffset, headPosition.forward);
            lHandRay = new Ray(lHandWeaponPos.position, -lHandWeaponPos.up);
            rHandRay = new Ray(rHandWeaponPos.position, -lHandWeaponPos.up);
        }

        private int PickUpRaycastsHandler()
        {
            faceRay.origin = headPosition.position + rayOffset;
            faceRay.direction = headPosition.forward;
            return Physics.RaycastNonAlloc(faceRay, results, headLength, interactionMask);
        }
        private void HandRaycastsHandler()
        {
            lHandRay.origin = lHandWeaponPos.position;
            lHandRay.direction = -lHandWeaponPos.up;
            rHandRay.origin = rHandWeaponPos.position;
            rHandRay.direction = -rHandWeaponPos.up;

            Physics.Raycast(lHandRay, out lHandHitInfo, handLength, interactionMask);
            Physics.Raycast(rHandRay, out rHandHitInfo, handLength, interactionMask);
        }

        private void AddWeapon()
        {
            int num = PickUpRaycastsHandler();
            for (int i = 0; i < num; i++)
            {
                results[i].transform.TryGetComponent(out BaseWeapon baseWeapon);
                
                if(baseWeapon == null || baseWeapon.isGet) continue;
                
                HasWeapon = true;
                DistributeWeaponPositions(baseWeapon.transform, baseWeapon);
                inventoryAddWeaponAction.Invoke(baseWeapon.weaponSo);
                return;
            }
        }
        
        private void DistributeWeaponPositions(Transform weaponTransform, BaseWeapon baseWeapon)
        {
            foreach (Transform targetPosition in weaponPositions)
            {
                if (targetPosition == null) Debug.Log("DJifj");
                if (!weaponsDic.TryAdd(targetPosition, baseWeapon)) continue;
                
                baseWeapon.isGet =  true;
                weaponTransform.SetParent(targetPosition, true);
                weaponTransform.position = targetPosition.position;
                weaponTransform.rotation = Quaternion.LookRotation(targetPosition.forward, Vector3.up);
                EquipWeapon();
                return;
            }

            Debug.Log("UI提示:"+"装备栏已满，无法获取");
        }
        private void EquipWeapon()
        {
            if (currentWeapon != null) return;
            
            int num = weaponPositions.Count;
            for (int i = 0; i < num; i++)
            {
                if (weaponsDic.TryGetValue(weaponPositions[i], out currentWeapon))
                {
                    damageModel = currentWeapon.GetWeaponComponent<DamageComponent>().GetDamageModel();
                    break;
                }
            }
        }

        private void DestroyToUI()
        {
            
        }
        public void ChangeCurrentWeapon()
        {
            for (int i = 0; i < weaponsDic.Count; i++)
            {
                
            }
        }
        
        private void RemoveWeapon(Transform key)
        {
            weaponsDic.Remove(key, out BaseWeapon weapon);
        } 

        private void OnInputPressed(string actionName)
        {
            if (actionName == "Interaction")
            {
                AddWeapon();
            }
        }

        public void WithdrawSword()
        {
            if(!HasWeapon) return;
            
            SheathComponent component = currentWeapon.GetWeaponComponent<SheathComponent>();
            if (component != null)
            {
                component.Draw(rHandWeaponPos);
            }
        }
        public void SheatheSword()
        {
            if(!HasWeapon) return;
            
            SheathComponent component = currentWeapon.GetWeaponComponent<SheathComponent>();
            if (component != null)
            {
                Debug.Log("SheatheSword");
                component.Sheathe();
            }
        }

        private void OnDisable()
        {
            InputManager.Instance.OnInputPressed -= OnInputPressed;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(headPosition.position + rayOffset, headPosition.forward * headLength);

            Gizmos.color = Color.green;
            Gizmos.DrawRay(lHandWeaponPos.position, -lHandWeaponPos.up * handLength);

            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(rHandWeaponPos.position, -rHandWeaponPos.up * handLength);
        }
    }
}