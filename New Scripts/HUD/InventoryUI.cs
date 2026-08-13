using UnityEngine;
using UnityEngine.UI;

namespace TNT
{
    public class InventoryUI : MonoBehaviour
    {
        public static InventoryUI singleton;
        public ItemContainer playerContainerUI; 
        public ItemContainer storageContainerUI;
        public GameObject inventoryPanel; 
        public InventoryItemUI itemUIPrefab;
        [Header("Drag and drop")] 
        public RectTransform dragDummy; 
        public Image dragIcon;

        public QuantitySelectUI quantitySelectUI; 
        public bool inventoryOpened;

        private void Awake() { singleton = this; }

        public void OpenPlayerContainer(Inventory playerInventory)
        {
            inventoryPanel.SetActive(true); 
            playerContainerUI.scrollView.SetActive(true); 
            playerContainerUI.dropPanel.SetActive(false);
            playerContainerUI.parentInventory = playerInventory; 
            Cursor.lockState = CursorLockMode.None; 
            inventoryOpened = true;
            UpdateContainerContent(playerContainerUI);
        }

        public void CloseInventory()
        {
            inventoryPanel.SetActive(false);
            storageContainerUI.scrollView.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked; 
            inventoryOpened = false; 
            storageContainerUI.parentInventory = null;
        }

        public void OpenStorageContainer(Inventory storage)
        {
            inventoryPanel.SetActive(true); 
            storageContainerUI.scrollView.SetActive(true); 
            storageContainerUI.dropPanel.SetActive(false);
            storageContainerUI.parentInventory = storage; 
            Cursor.lockState = CursorLockMode.None; inventoryOpened = true;
            UpdateContainerContent(storageContainerUI);
        }

        public void UpdatePlayerContent(Inventory inventory) 
        { 
            playerContainerUI.parentInventory = inventory; 
            UpdateContainerContent(playerContainerUI); 
        }
        public void UpdateStorageContent(Inventory inventory) 
        { 
            storageContainerUI.parentInventory = inventory; 
            UpdateContainerContent(storageContainerUI); 
        }

        private void UpdateContainerContent(ItemContainer container)
        {
            for (int i = 0; i < container.itemParent.childCount; i++) Destroy(container.itemParent.GetChild(i).gameObject);
            if (container.parentInventory == null) return;
            for (int i = 0; i < container.parentInventory.storedItems.Count; i++)
            {
                ItemData data = container.parentInventory.database.GetItemFromID(container.parentInventory.storedItems[i].ItemDataID);
                InventoryItemUI newItem = Instantiate(itemUIPrefab, container.itemParent);
                newItem.itemNameText.text = data.itemName; newItem.quantityText.text = container.parentInventory.storedItems[i].quantity.ToString();
                newItem.itemIcon.sprite = data.itemIcon; newItem.inventoryItemIndex = i; newItem.parentInventory = container.parentInventory; newItem.playerUI = this;
            }
        }

        public void SetDropPanelActive(bool isActive) 
        { 
            playerContainerUI.dropPanel.SetActive(isActive); 
            storageContainerUI.dropPanel.SetActive(isActive); 
        }
    }

    [System.Serializable]
    public struct ItemContainer { public GameObject scrollView; public Transform itemParent; public GameObject dropPanel; public Inventory parentInventory; }
}