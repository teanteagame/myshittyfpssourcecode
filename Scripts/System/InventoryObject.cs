using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryObject : MonoBehaviour
{
    public float maxCapacity = 100;
    protected float currentSpaceTaken = 0;

    public List<ItemInstance> storedItems = new List<ItemInstance>();

    public virtual void AddItem(ItemData data, int quantity, CustomItemProperties[] properties = null)
    {
        ///Add new item
        ///Check if there is room in inventory
        ///If the item can be stacked, find an empty stack in the inventory, if not, create a new stack if there is enough room in the inventory
        ///If there is not enough room, remove the number being added to make room

        UpdateInventory();
    }

    public virtual void RemoveItem(ItemInstance item, int quantity)
    {
        ///Remove item
        ///Remove completely from inventory

        UpdateInventory();
    }

    public virtual void DropItem(ItemInstance item, int quantity)
    {
        ///Drop Item
        ///Delete the selected quantity of the selected ItemInstance
        ///Create a dropPrefab of the dropped item

        UpdateInventory();
    }

    public virtual void  EquipItem(ItemInstance item, EquipmentPanelUI equipSlot)
    {
        ///Equip item
        /// The item will be equipped in the equipment slot
        /// Check if the item type is correct for the item type allowed by the equipment slot, otherwise drop the item
        ///If the item is a stack (e.g. consumable) then only take 1 unit of that stack into the equipment slot, the rest will be kept in the inventory and the unit in the equipment slot that is used up will automatically equip the same item if there is still in the inventory
        UpdateInventory();
    }

    public virtual void UpdateInventory()
    {
        ///Check if there are any unreasonable ItemInstances (e.g. quantity is 0 or greater than maximum quantity, data is not assigned or lost,)
        //Check if inventory is overflowed (number of items is greater than allowed) then we will discard items at the end of inventory list
    }

    public ItemInstance GetPossibleStackSlot(ItemData itemData) 
    {
        foreach (ItemInstance item in storedItems) 
        {
            if(item.data == itemData && item.quantity < item.data.maxStackSize)
            {
                return item;
            }
        }
        return null;
    }

    public bool HaveSpaceLeft()
    {
        return currentSpaceTaken < maxCapacity;
    }

    public bool ThisFitInventory(float spaceTaken)
    {
        return spaceTaken  <=  (maxCapacity - currentSpaceTaken);
    }
}

[System.Serializable]
public class CustomItemProperties
{
    public string Name;
    public float Value;

    public CustomItemProperties(string name, float value)
    {
        this.Name = name;
        this.Value = value;
    }
}

[System.Serializable]
public class ItemInstance
{
    public ItemData data;
    public int quantity;
    public CustomItemProperties[] properties;

    public ItemInstance(ItemData data, int quantity, CustomItemProperties[] properties)
    {
        this.data = data;
        this.quantity = quantity;
        this.properties = properties;
    }
}