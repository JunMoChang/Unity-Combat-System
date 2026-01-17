using System;
using System.Collections.Generic;
using Player;
using Scriptable;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Inventory
{
    public class InventoryManager : MonoBehaviour
    {
        public static InventoryManager Instance {get; private set; }
        
        [SerializeField] private SlotContainerCategory slotContainerCategory;
        
        [SerializeField] private ItemSlotsSort itemSlotsSort;
        
        private readonly Dictionary<ItemScriptableObject.ItemType, SlotContainer> itemSlotContainersDic = new ();
        public SlotContainer[] inventorySlotsContainers;
        private SlotContainer allSlotContainer;
        
        [SerializeField] private ItemScriptableObject[] itemSos;
        private ItemSlot currentSelectedItemSlot;
        

        public Action<ItemScriptableObject> discardItemAction;
        public Action<ItemSlot> setReferenceSlotAction;
        
        private void Awake()
        {
            if(Instance == null) Instance = this;
            else Destroy(gameObject);
            
            foreach (SlotContainer slot in inventorySlotsContainers)
            {
                itemSlotContainersDic.Add(slot.itemType, slot);
            }
            
            itemSlotContainersDic.TryGetValue(ItemScriptableObject.ItemType.All, out allSlotContainer);
        }

        private void Start()
        {
            slotContainerCategory.RegisterButtonActions(inventorySlotsContainers);
            
            itemSlotsSort.Initialize(itemSlotContainersDic);
            itemSlotsSort.RegisterButtonAction();
            
            itemSlotsSort.CurrentSlotContainer = slotContainerCategory.GetCurrentSlotContainer();

            slotContainerCategory.OnSlotContainerChanged += container =>
            {
                itemSlotsSort.CurrentSlotContainer = container;
            };
        }

        private void OnEnable()
        {
            PlayerController.Instance.weaponHandel.inventoryAddWeaponAction += AddItem;
        }

        private void OnDisable()
        {
            PlayerController.Instance.weaponHandel.inventoryAddWeaponAction -= AddItem;
        }
        
        public bool UseItem(ItemScriptableObject targetItemSos)
        {
            int num = itemSos.Length;
            for (int i = 0; i < num; i++)
            {
                if (itemSos[i].itemType == targetItemSos.itemType && itemSos[i].itemName == targetItemSos.itemName)
                {
                    return itemSos[i].UseItem();
                }
            }
            return false;
        }
        
        public int AddItem(ItemScriptableObject itemSo)
        {
            SlotContainer itemSlotContainer = SelectedSlotContainer(itemSo.itemType);
            if(itemSlotContainer == null) return itemSo.itemQuantity;
        
            return itemSlotContainer.AddItem(itemSo);
        }

        public void DiscardItem(ItemScriptableObject itemSo)
        {
            discardItemAction?.Invoke(itemSo);
        }
        
        public void CreateReferenceInAllContainer(ItemSlot sourceSlot)
        {
            ItemSlot[] itemSlots = allSlotContainer.GetItemSlots();

            foreach (ItemSlot slot in itemSlots)
            {
                if (slot.storageItemQuantity != 0) continue;
            
                slot.SetAsReferenceSlot(sourceSlot);
                
                return;
            }
        }

        public void TriggerSetReferenceSlotAction(ItemSlot sourceSlot)
        {
            setReferenceSlotAction?.Invoke(sourceSlot);
        }
        public void SetCurrentSelectedSlot(ItemSlot slot)
        {
            if (currentSelectedItemSlot == slot) return;
        
            if(currentSelectedItemSlot != null) currentSelectedItemSlot.SetDeselected();
        
            currentSelectedItemSlot = slot;
        }
    
        public void DeSelectedAllItems(ItemScriptableObject.ItemType itemType)
        {
            SlotContainer slotContainer = SelectedSlotContainer(itemType);
            if(slotContainer == null) return;
        
            slotContainer.DeSelectedAllItems();
        }
    
        private SlotContainer SelectedSlotContainer(ItemScriptableObject.ItemType itemType)
        {
            itemSlotContainersDic.TryGetValue(itemType, out SlotContainer slotContainer);
            return slotContainer;
        }

        public SlotContainer GetCurrentSlotsContainer( )
        {
            return slotContainerCategory.GetCurrentSlotContainer();
        }
        
    }
}
