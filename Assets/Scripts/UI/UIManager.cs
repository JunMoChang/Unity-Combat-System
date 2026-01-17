using System;
using System.Collections.Generic;
using Player;
using UI.Inventory;
using UI.Inventory.Equipment;
using UnityEngine;


namespace UI
{
    public class UIManager :  MonoBehaviour
    {
        [NonSerialized] public InventoryManager  inventoryManager;
        [NonSerialized] public EquipmentUIController  equipmentUIController;

        private MenuData currentMenu;
        private Dictionary<InputManager.UIActionName, MenuData> menus = new();
        
        public GameObject inventoryUI;
        public GameObject equipmentUI;

        private class MenuData
        {
            private readonly GameObject menuUI;

            public MenuData(GameObject ui)
            {
                menuUI = ui;
            }
            
            public void OpenMenu()
            {
                menuUI.SetActive(true);
                Cursor.lockState = CursorLockMode.Confined;
            }

            public void CloseMenu()
            {
                menuUI.gameObject.SetActive(false);
                Cursor.lockState = CursorLockMode.Locked; 
            }
        }

        void Start()
        {
            inventoryManager =  InventoryManager.Instance;
            equipmentUIController = EquipmentUIController.Instance;
            
            menus = new Dictionary<InputManager.UIActionName, MenuData>
            {
                {InputManager.UIActionName.OpenInventory, new MenuData(inventoryUI)},
                {InputManager.UIActionName.OpenEquipment, new MenuData (equipmentUI)}
            };

            InputManager.Instance.OnInputPressed += HandleUIInput;
        }

        private void HandleUIInput(string actionName)
        {
            if (!Enum.TryParse<InputManager.UIActionName>(actionName, out var uiActionName)) return;
            
            if (menus.TryGetValue(uiActionName, out MenuData newMenu) || uiActionName == InputManager.UIActionName.Back)
            {
                ToggleMenu(newMenu);
            }
        }

        private void ToggleMenu(MenuData newMenu)
        {
            if (currentMenu == newMenu)
            {
                currentMenu?.CloseMenu();
                currentMenu = null;
            }
            else
            {
                currentMenu?.CloseMenu();
                currentMenu = newMenu;
                currentMenu?.OpenMenu();
            }
        }
    }
}