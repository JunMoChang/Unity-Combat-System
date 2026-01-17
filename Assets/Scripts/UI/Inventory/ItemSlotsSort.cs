using System;
using System.Collections.Generic;
using System.Linq;
using Scriptable;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Inventory
{
    [Serializable]
    public class ItemSlotsSort
    {
        public SlotContainer CurrentSlotContainer{get; set;}
        
        private ItemSortType currentSortType;
        private SortOrder currentSortOrder;
        
        [SerializeField] private List<string> options;
        [SerializeField] private Dropdown sortTypeDropdown;
        [SerializeField] private TextMeshProUGUI sortOrderText;
        [SerializeField] private Button sortOrderButton;

        private Dictionary<ItemScriptableObject.ItemType, SlotContainer> slotContainers;
        
        class ItemInfo
        {
            public ItemScriptableObject itemSo;
            public int storageItemQuantity;
            public ItemSlot sourceSlot;
        }

        public void Initialize(Dictionary<ItemScriptableObject.ItemType, SlotContainer> containers)
        {
            slotContainers = containers;
        }
        
        public void RegisterButtonAction()
        {
            if (sortTypeDropdown != null)
            {
                sortTypeDropdown.ClearOptions();
                
                sortTypeDropdown.AddOptions(options);
                sortTypeDropdown.onValueChanged.AddListener(ChangeSortType);
            }
            sortOrderButton.onClick.AddListener(ChangeSortOrder);
            currentSortOrder = SortOrder.Ascending;
        }

        private void ChangeSortType(int index)
        {
            currentSortType = (ItemSortType)index;
            SortCurrentContainer();
        }

        private void ChangeSortOrder()
        {
            currentSortOrder = (currentSortOrder == SortOrder.Ascending) ? SortOrder.Descending : SortOrder.Ascending;
        
            sortOrderText.text = currentSortOrder == SortOrder.Ascending ? "↑升序" : "↓降序";
            SortCurrentContainer();
        }
    
        private void SortCurrentContainer()
        {
            if(currentSortType == ItemSortType.None) return;
            
            if(CurrentSlotContainer == null) return;
        
            if (CurrentSlotContainer.itemType == ItemScriptableObject.ItemType.All)
            {
                SortAllSlotsContainer();
                return;
            }

            SortOtherSlotsContainer(CurrentSlotContainer);
        }
        
        private void SortAllSlotsContainer()
        {
            List<ItemInfo> allSortedItemSlots = new ();
            SlotContainer allSlotContainer = null;
            
            foreach (SlotContainer container in slotContainers.Values)
            {
                if (container.itemType == ItemScriptableObject.ItemType.All)
                {
                    allSlotContainer = container;
                    continue;
                }
            
                ItemSlot[] slots = container.GetItemSlots();
                foreach (ItemSlot slot in slots)
                {
                    if (slot.storageItemQuantity > 0)
                    {
                        allSortedItemSlots.Add(new ItemInfo
                        {
                            itemSo = slot.itemSo,
                            storageItemQuantity = slot.storageItemQuantity,
                            sourceSlot = slot
                        });
                    }
                }
            }
            if(allSlotContainer == null) return;
        
            allSortedItemSlots = ItemSortByType(allSortedItemSlots);
        
            ItemSlot[] allContainerSlots = allSlotContainer.GetItemSlots();
            foreach (ItemSlot slot  in allContainerSlots)
            {
                slot.EmptySlot();
            }
            
            for (int i = 0; i < allContainerSlots.Length && i < allSortedItemSlots.Count; i++)
            {
                allContainerSlots[i].SetAsReferenceSlot(allSortedItemSlots[i].sourceSlot);
            }
        }

        private void SortOtherSlotsContainer(SlotContainer container)
        {
            ItemSlot[] slots = container.GetItemSlots();

            List<ItemInfo> sortedSlots = new ();
            foreach (ItemSlot slot in slots)
            {
                if(slot.storageItemQuantity <= 0) continue;

                sortedSlots.Add(new ItemInfo
                {
                    itemSo = slot.itemSo,
                    storageItemQuantity =  slot.storageItemQuantity,
                });
            }
            if(sortedSlots.Count == 0) return;
            
            foreach (ItemSlot slot in slots)
            {
                slot.EmptySlot();
            }
        
            sortedSlots = ItemSortByType(sortedSlots);
           
            for (int i = 0; i < sortedSlots.Count; i++)
            {
                slots[i].AddItem(sortedSlots[i].itemSo, sortedSlots[i].itemSo.itemQuantity);
            }
        }
        
        private List<ItemInfo> ItemSortByType(List<ItemInfo> sortedSlots)
        {
            List<ItemInfo> resultSortedList = new (sortedSlots);
        
            switch (currentSortType)
            {
                case ItemSortType.Name:
                    resultSortedList =  resultSortedList.OrderBy(t => t.itemSo.itemName).ToList();
                    break;
                case ItemSortType.Type:
                    resultSortedList = resultSortedList.OrderBy(t => t.itemSo.itemType).ToList();
                    break;
                case ItemSortType.Quantity:
                    resultSortedList = resultSortedList.OrderBy(t => t.storageItemQuantity).ToList();
                    break;
                case ItemSortType.Rarity:
                    resultSortedList = resultSortedList.OrderBy(t => t.itemSo.itemRarity).ToList();
                    break;
                default: return null;
            }

            if (currentSortOrder == SortOrder.Descending)
            {
                resultSortedList.Reverse();
            }
        
            return resultSortedList;
        }
    }
}
