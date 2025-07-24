using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerInventory : InventoryObject
{
    private PlayerUI playerUI;

    private void Start()
    {
        playerUI = GetComponent<PlayerUI>();
        UpdateInventory();
    }

    public override void AddItem(ItemData data, int quantity, CustomItemProperties[] properties = null)
    {
        if (data == null || quantity <= 0) return;
        float spaceTaken = quantity * data.weightPerUnit;
        bool isStackable = data.maxStackSize > 1 ? true : false;

        if (isStackable) 
        {
            //Find possible stack
            ItemInstance possibleSlot = GetPossibleStackSlot(data);
            if (possibleSlot != null)
            {
                int quantityLeft = possibleSlot.data.maxStackSize - possibleSlot.quantity;
                if(quantityLeft >= quantity)
                {
                    possibleSlot.quantity += quantity;
                }
                else
                {
                    possibleSlot.quantity += quantityLeft;
                    AddItem(data, quantity - quantityLeft, properties);
                }
            }
            else
            {
                ItemInstance newItem = new ItemInstance(data, quantity, properties);
                if (ThisFitInventory(spaceTaken))
                {
                    storedItems.Add(newItem);
                }
                else
                {
                    DropItem(newItem, newItem.quantity);
                }
            }
        }
        else
        {
            ItemInstance newItem = new ItemInstance(data, quantity, properties);
            if (ThisFitInventory(spaceTaken))
            {
               storedItems.Add(newItem);
            }
            else
            {
                DropItem(newItem, newItem.quantity);
            }
        }
        UpdateInventory();
    }

    public override void RemoveItem(ItemInstance item, int quantity)
    {
        item.quantity -= item.quantity > quantity ? quantity : item.quantity;

        UpdateInventory();
    }

    public override void DropItem(ItemInstance item, int quantity)
    {
        UpdateInventory();
    }

    public override void UpdateInventory()
    {
        storedItems.RemoveAll(item => item == null || item.data == null || item.quantity <= 0);

        for (int i = 0; i < storedItems.Count; i++)
        {
            ItemInstance item = storedItems[i];

            if (item.quantity > item.data.maxStackSize)
            {
                item.quantity = item.data.maxStackSize;
            }
        }

        currentSpaceTaken = 0;

        for (int i = 0; i < storedItems.Count; i++)
        {
            ItemInstance item = storedItems[i];

            currentSpaceTaken += item.quantity * item.data.weightPerUnit;
        }

        playerUI.RefreshInventoryUI();
    }

   
}



