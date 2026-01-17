using Scriptable;
using UnityEngine;

namespace UI.Inventory
{
    public class Item : MonoBehaviour
    {
        public ItemScriptableObject itemSo;
    
        private InventoryManager inventoryManager;
        void Start()
        {
            inventoryManager = InventoryManager.Instance;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.collider.CompareTag("Player"))
            {
                int extraItems = inventoryManager.AddItem(itemSo);
            
                if(extraItems > 0)
                {
                    itemSo.itemQuantity = extraItems;
                }
            }
        }
    }
}
