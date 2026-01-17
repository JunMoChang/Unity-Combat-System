using System;
using Player;
using Scriptable;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using WeaponConfiguration.AbstractWeapon;

namespace UI.Inventory
{
    public class ItemSlot : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private Vector3 offset = new (0, 0, -2); 
        [NonSerialized]public int storageItemQuantity;
        [NonSerialized] public bool isFull;
        [NonSerialized] public ItemScriptableObject itemSo;
        public Sprite emptySprite;
        public Sprite defaultItemSprite;
        private int itemMaxSuperposition;
    
        [SerializeField] private TMP_Text itemSlotQuantityText;
        [SerializeField] private Image itemSlotImage;
    
        [SerializeField] private TMP_Text itemDescriptionNameText;
        [SerializeField] private TMP_Text itemDescriptionText;
        [SerializeField] private Image itemDescriptionImage;

        public GameObject selectedPanel;
        [NonSerialized]public bool isSelected;
    
        private InventoryManager inventoryManager;
    
        private bool isReferenceSlot;
        private ItemSlot sourceSlot;
        private readonly System.Collections.Generic.List<ItemSlot> referenceSlots = new ();
        
        void Start()
        {
            inventoryManager = InventoryManager.Instance;
            
            if(itemDescriptionImage != null) itemDescriptionImage.sprite = emptySprite;
        }
    
        public int AddItem(ItemScriptableObject targetItemSo, int  quantity)
        {
            if (isFull) return targetItemSo.itemQuantity;

            if (quantity < 0 || quantity > targetItemSo.itemQuantity) quantity = targetItemSo.itemQuantity;
                
            bool wasEmpty = storageItemQuantity == 0;
            
            itemSo = targetItemSo;
            itemSlotImage.sprite = itemSo.itemIcon;
            
            itemMaxSuperposition = targetItemSo.itemMaxSuperposition;
            storageItemQuantity += quantity;
            
            int extraItems = 0;

            if (storageItemQuantity > itemMaxSuperposition)
            {
                itemSlotQuantityText.text = itemMaxSuperposition.ToString();
                itemSlotQuantityText.enabled = true;
                isFull = true;
                
                extraItems = storageItemQuantity - itemMaxSuperposition;
                storageItemQuantity = itemMaxSuperposition;
            }

            if (wasEmpty)
            {
                inventoryManager.CreateReferenceInAllContainer(this);
                
                if (targetItemSo.itemType == ItemScriptableObject.ItemType.Weapon) 
                    inventoryManager.TriggerSetReferenceSlotAction(this);
            }
            
            
            SyncToReferenceSlots();
            
            return extraItems;
        }
        
        public void OnPointerClick(PointerEventData eventData)
        {
            switch (eventData.button)
            {
                case PointerEventData.InputButton.Left:
                    OnLeftClick();
                    break;
                case PointerEventData.InputButton.Right:
                    OnRightClick();
                    break;
                case PointerEventData.InputButton.Middle:
                    break;
                default:
                    return;
            }
        }
        private void OnLeftClick()
        {
            if (isReferenceSlot && sourceSlot != null)
            {
                sourceSlot.SelectedAndUse();
                /*sourceSlot.OnPointerClick(new PointerEventData(EventSystem.current) 
            { 
                button = PointerEventData.InputButton.Left 
            });*/
                return;
            }
            SelectedAndUse();
        }
        private void OnRightClick()
        {
            if (!isSelected || storageItemQuantity == 0) return;
        
            if (isReferenceSlot && sourceSlot != null)
            {
                sourceSlot.DiscardItem();
                /*sourceSlot.OnPointerClick(new PointerEventData(EventSystem.current)
            { 
                button = PointerEventData.InputButton.Right 
            });*/
            
                return;
            }
            DiscardItem();
        }

        private void SelectedAndUse()
        {
            if (isSelected)
            {
                if(itemSo == null) return;
            
                bool usable = inventoryManager.UseItem(itemSo);
                if (!usable) return;
            
                storageItemQuantity -= 1;
                if (storageItemQuantity <= 0)
                {
                    EmptySlot();
                    return;
                }
            
                itemSlotQuantityText.text = storageItemQuantity.ToString();
                itemSlotQuantityText.enabled = true;
                isFull = false;

                SyncToReferenceSlots();
                return;
            }
        
            if(itemSo == null) return;
        
            inventoryManager.SetCurrentSelectedSlot(this);
            isSelected = true;
            selectedPanel.SetActive(isSelected);
        
            itemDescriptionNameText.text = itemSo.itemName;
            itemDescriptionText.text = itemSo.itemDescription;
            itemDescriptionImage.sprite = itemSo.itemIcon;
            if(itemDescriptionImage.sprite == null) itemDescriptionImage.sprite = emptySprite;
        
            SyncToReferenceSlots();
        }
        private void DiscardItem()
        {
            CreateItem();
            storageItemQuantity -= 1;
            if (storageItemQuantity <= 0)
            {
                EmptySlot();
                return;
            }
            itemSlotQuantityText.text = storageItemQuantity.ToString();
            itemSlotQuantityText.enabled = true;
            isFull = false;
        
            SyncToReferenceSlots();
        }
        private void CreateItem()
        {
            if (itemSo.prefab == null) return;
            
            GameObject item = Instantiate(itemSo.prefab);
            item.name = itemSo.itemName;
            Transform player =  PlayerController.Instance.transform;
            item.transform.rotation = player.rotation;
            item.transform.position = player.position + player.TransformDirection(offset);
                
            switch (itemSo.itemType)
            {
                case ItemScriptableObject.ItemType.Weapon:
                    item.GetComponent<BaseWeapon>().weaponSo =  itemSo;
                    break;
                default: return;
            }
        }
        
        public void EmptySlot()
        {
            ClearDisplay();
            ClearReferenceRelationships();
        }
        private void ClearDisplay()
        {
            itemMaxSuperposition = 0;
            storageItemQuantity = 0;
            itemSo = null;
        
            itemSlotImage.sprite = defaultItemSprite;
            itemSlotQuantityText.enabled = false;

            if (itemDescriptionImage != null)
            {
                itemDescriptionImage.sprite = emptySprite;
                itemDescriptionText.text = "";
                itemDescriptionNameText.text = "";
            }
        }
        private void ClearReferenceRelationships()
        {
            if (referenceSlots.Count > 0)
            {
                foreach (ItemSlot referenceSlot in referenceSlots)
                {
                    if (referenceSlot != null)
                    {
                        referenceSlot.sourceSlot =  null;
                        referenceSlot.isReferenceSlot = false;
                        referenceSlot.ClearDisplay();
                    }
                }
            }
        
            if (isReferenceSlot && sourceSlot != null)
            {
                sourceSlot.referenceSlots.Remove(this);
                sourceSlot = null;
                isReferenceSlot = false;
            }
        }
    
        private void SyncToReferenceSlots()
        {
            System.Collections.Generic.List<ItemSlot> referenceSlotsCopy = new (referenceSlots);

            foreach (ItemSlot refSlot in referenceSlotsCopy)
            {
                if (refSlot == null) continue;
            
                refSlot.SyncFromSourceSlot(this);
            }
        }
        private void SyncFromSourceSlot(ItemSlot targetSlot)
        {
            if (targetSlot.storageItemQuantity <= 0)
            {
                EmptySlot();
            }
            else
            {
                itemSo = targetSlot.itemSo;
                storageItemQuantity = targetSlot.storageItemQuantity;
                isFull = targetSlot.isFull;
                isSelected =  targetSlot.isSelected;
            
                selectedPanel.SetActive(isSelected);
                itemSlotImage.sprite = itemSo.itemIcon;
                itemSlotQuantityText.text = storageItemQuantity.ToString();
                itemSlotQuantityText.enabled = true;
            }
        }
        public void SetAsReferenceSlot(ItemSlot targetSlot)
        {
            isReferenceSlot = true;
            sourceSlot = targetSlot;
        
            if (!targetSlot.referenceSlots.Contains(this))
            {
                targetSlot.referenceSlots.Add(this);
            }
        
            SyncFromSourceSlot(targetSlot);
        }
    
        public void SetDeselected()
        {
            isSelected = false;
            selectedPanel.SetActive(false);
        
            SyncToReferenceSlots();
        }
    }
}
