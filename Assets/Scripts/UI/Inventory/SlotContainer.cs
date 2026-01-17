using Scriptable;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Inventory
{
    public class SlotContainer : MonoBehaviour
    {
        public ItemScriptableObject.ItemType itemType;
    
        [SerializeField] private ItemSlot[] itemSlots;
        
        [SerializeField]private ScrollRect scrollRect;
        
        void Start()
        {
            if(itemType != ItemScriptableObject.ItemType.All) gameObject.SetActive(false);
        }

        void OnEnable()
        {
            ResetScroll();
        }
        
        public int AddItem(ItemScriptableObject itemSo)
        {
            int currenQuantities = itemSo.itemQuantity;

            while (currenQuantities > 0)
            {
                bool isFound = false;
                foreach (ItemSlot itemSlot in itemSlots)
                {
                    if (itemSlot.itemSo == null || !itemSlot.isFull && itemSlot.itemSo.itemName == itemSo.itemName)
                    {
                        isFound = true;
                        
                        int extraItems = itemSlot.AddItem(itemSo, currenQuantities);
                        
                        currenQuantities = extraItems;
                        break;
                    }
                }
                
                if (!isFound) break;
            }
            
            return currenQuantities;
        }

        public void DiscardItem(ItemScriptableObject itemSo)
        {
            
        }
        public void DeSelectedAllItems()
        {
            int num = itemSlots.Length;
            for (int i = 0; i < num; i++)
            {
                itemSlots[i].selectedPanel.SetActive(false);
                itemSlots[i].isSelected = false;
            }
        }

        public ItemSlot[] GetItemSlots()
        {
            return itemSlots;
        }
        
        private void ResetScroll()
        {
            if (scrollRect != null) scrollRect.verticalNormalizedPosition = 1f;
        }
    }
}
