using System;
using System.Collections.Generic;
using Scriptable;
using UnityEngine;

namespace UI.Inventory.Equipment
{
    public class EquipmentUIController : MonoBehaviour
    {
        public static EquipmentUIController Instance {get; private set;}
        
        [SerializeField] private SlotContainerCategory slotContainerCategory;
        [SerializeField] private ItemSlotsSort itemSlotsSort; 
        
        [SerializeField] private SlotContainer[]  slotContainers;
        private readonly Dictionary<ItemScriptableObject.ItemType, SlotContainer> itemSlotContainersDic = new ();
        
        public Action<ItemScriptableObject> equipWeaponAction;
        
        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }
        
        private void Start()
        {
            InventoryManager.Instance.setReferenceSlotAction += CreateReferenceSlot;

            foreach (SlotContainer slotContainer in slotContainers)
            {
                itemSlotContainersDic.Add(slotContainer.itemType, slotContainer);
            }
            
            slotContainerCategory.RegisterButtonActions(slotContainers);

            itemSlotsSort.Initialize(itemSlotContainersDic);
            itemSlotsSort.RegisterButtonAction();
            
            itemSlotsSort.CurrentSlotContainer = slotContainerCategory.GetCurrentSlotContainer();
            slotContainerCategory.OnSlotContainerChanged += container =>
            {
                itemSlotsSort.CurrentSlotContainer = container;
            };
        }
        
        
        private void CreateReferenceSlot(ItemSlot sourceSlot)
        {
            foreach (SlotContainer container in slotContainers)
            {
                if (container.itemType != sourceSlot.itemSo.itemType) continue;
                
                ItemSlot[] itemSlots = container.GetItemSlots();

                foreach (ItemSlot slot in itemSlots)
                {
                    if(slot.storageItemQuantity != 0) continue;
                        
                    slot.SetAsReferenceSlot(sourceSlot);
                        
                    return;
                }
            }
        }
    }
}