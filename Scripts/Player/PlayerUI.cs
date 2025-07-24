using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [Header("PlayerInteract")]
    public GameObject interactTooltip;
    public TextMeshProUGUI interactText;
    public Slider interactProcessFill;

    [Header("Player Inventory")]
    public GameObject inventoryUI;
    public InventoryItemUI itemUIPrefab;
    public RectTransform onDragLayer;

    [Header("Inventory Window")]
    public GameObject inventoryWindow;
    public RectTransform inventoryContainer;
    public InventoryPanelUI inventoryPanelUI;

    [Header("Equipment Window")]
    public GameObject equipmentWindow;
    public EquipmentPanelUI helmetSlot;
    public EquipmentPanelUI vestSlot;
    public EquipmentPanelUI backpackSlot;
    public EquipmentPanelUI primarySlot;
    public EquipmentPanelUI secondarySlot;
    public EquipmentPanelUI handgunSlot;
    public EquipmentPanelUI shealthSlot;
    public EquipmentPanelUI throwableSlot;

    [Header("LootWindow")]
    public GameObject lootWindow;
    public RectTransform lootContainer;
    public InventoryPanelUI lootPanelUI;

    private PlayerInventory inventory;

    private void Start()
    {
        inventory = GetComponent<PlayerInventory>();
        inventoryPanelUI.parentInventory = inventory;
    }

    public void UpdateInteractUI(string interactMsg, float interactTime, float maxInteractTime, bool isTrue, bool input)
    {
        interactTooltip.SetActive(isTrue);
        interactText.SetText(interactMsg);
        interactProcessFill.gameObject.SetActive(input);
        interactProcessFill.value = interactTime;
        interactProcessFill.maxValue = maxInteractTime;
    }

    public void OpenInventory()
    {
        inventoryUI.gameObject.SetActive(true);
        RefreshInventoryUI();
    }

    public void RefreshInventoryUI()
    {
        for (int i = 0; i < inventoryContainer.childCount; i++) 
        {
            Destroy(inventoryContainer.GetChild(i).gameObject);
        }

        for (int i = 0; i < inventory.storedItems.Count; i++) 
        {
            ItemInstance item = inventory.storedItems[i];

            InventoryItemUI newItemUI = Instantiate(itemUIPrefab, inventoryContainer);
            newItemUI.parentTransform = inventoryContainer;
            newItemUI.parentInventory = inventory;
            newItemUI.inventoryItemIndex = i;
            newItemUI.playerUI = this;
            newItemUI.itemNameText.SetText(item.data.itemName);
            newItemUI.quantityText.SetText(item.quantity > 1 ? item.quantity.ToString() : string.Empty);
            newItemUI.itemIcon.sprite = item.data.icon;
        }
    }

    public void CloseInventory()
    {
        inventoryUI.gameObject.SetActive(false);
    }

    public void RefreshLootUI()
    {

    }

    public void EnableInventoryPanel(bool enable)
    {
        if (enable)
        {
            inventoryPanelUI.gameObject.SetActive(true);

            if (lootWindow.activeInHierarchy)
            {
                lootPanelUI.gameObject.SetActive(true);
            }
        }
        else
        {
            inventoryPanelUI.gameObject.SetActive(false);
            lootPanelUI.gameObject.SetActive(false);
        }
    }
}
