using UI.Inventory;
using UnityEngine;
using UnityEngine.Serialization;

namespace Scriptable
{
    [CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
    public class ItemScriptableObject : ScriptableObject
    {
        [Header("基础信息")]
        public string itemName;
        public ItemType itemType;
        public int itemQuantity;
        public int itemMaxSuperposition;
        [FormerlySerializedAs("rarity")] public ItemRarity itemRarity;
        public Sprite itemIcon;
        [TextArea(3, 5)]
        public string itemDescription;
        public GameObject prefab;
        
        [Header("消耗品属性")]
        [Tooltip("消耗品类型配置")]
        public BaseState baseState;
        public int amountRecover;
        
        [Header("Buff效果")]
        public Buff buff;
        public int amountBuff;
        public float buffDuration;
        
        [Header("武器属性")]
        [Tooltip("武器类型配置")]
        public int weaponDamage;
        public float attackSpeed = 1f;
        public float attackRange = 1.5f;
    
        public enum ItemType
        {
            All,
            Weapon,
            Armor,
            Consumable,
            Material,
            Other
        }
        public enum BaseState
        {
            None,
            Health
        }

        public enum Buff
        {
            None,
            MaxHealth,
            Speed,
            Damage,
        }

        public bool UseItem()
        {
            if (baseState == BaseState.Health)
            {
                Debug.Log($"血量加{amountRecover}");
                return true;
            }
            return false;
        }
    }
}
