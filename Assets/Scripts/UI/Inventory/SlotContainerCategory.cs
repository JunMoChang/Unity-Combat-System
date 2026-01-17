using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Inventory
{
    [Serializable]
    public class SlotContainerCategory
    {
        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] private Button[] categoryButtons;
        
        private SlotContainer currentSlotContainer;
        
        public event Action<SlotContainer> OnSlotContainerChanged;
        
        public void RegisterButtonActions(SlotContainer[] slotContainers)
        {
            int length = Math.Min(categoryButtons.Length, slotContainers.Length);
            for (int i = 0; i < length; i++)
            {
                int index = i;
                categoryButtons[i].onClick.AddListener(() => CategoryButtonClick(slotContainers[index]));
            }

            currentSlotContainer = slotContainers[0];
            CategoryButtonClick(currentSlotContainer);
        }
        
        private void CategoryButtonClick(SlotContainer slotContainer)
        {
            if(slotContainer.gameObject.activeSelf) return;
            currentSlotContainer.DeSelectedAllItems();
            currentSlotContainer.gameObject.SetActive(false);
            
            currentSlotContainer = slotContainer;
            scrollRect.content = currentSlotContainer.gameObject.GetComponent<RectTransform>();
            currentSlotContainer.gameObject.SetActive(true);
            
            OnSlotContainerChanged?.Invoke(currentSlotContainer);
        }

        public SlotContainer GetCurrentSlotContainer()
        {
            return currentSlotContainer;
        }
    }
}
