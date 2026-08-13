using UnityEngine;

namespace TNT
{
    public class PlayerInventory : Inventory
    {
        private InventoryUI inventoryUI;
        private PlayerInputs inputs;

        public override void Start()
        {
            base.Start();
            inventoryUI = InventoryUI.singleton;
            inputs = GetComponent<PlayerInputs>();
            inventoryUI.playerContainerUI.parentInventory = this;
        }

        private void Update()
        {
            if (inputs.inventory.pressed)
            {
                inputs.inventory.pressed = false;
                if (inventoryUI.inventoryOpened)
                {
                    inventoryUI.CloseInventory();
                }
                else
                {                    
                    inventoryUI.OpenPlayerContainer(this);
                }
            }

            if (Input.GetKeyDown(KeyCode.Y))
            {
                ItemData randomItem = database.items[Random.Range(0, database.items.Count)];
                int randomQuantity = randomItem.maxQuantityPerStack > 1 ? Random.Range(1, randomItem.maxQuantityPerStack) : 1;
                Debug.Log("Added " + randomItem.itemName + " with " + randomQuantity);
                AddItem(randomItem.ItemID, randomQuantity, null);
                
            }            
        }

        public override void UpdateInventory()
        {
            base.UpdateInventory();
            inventoryUI.UpdatePlayerContent(this);
        }
    }
}
