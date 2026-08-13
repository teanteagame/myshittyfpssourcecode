using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TNT
{
    public class Inventory : MonoBehaviour
    {
        public Database database; public float capacity = 100f; public float takenSpace = 0f;
        public List<ItemInstance> storedItems = new();
        public Vector3 dropOffset;

        public virtual void Start() { database = Database.singleton; }

        public virtual void AddItem(string itemID, int quantity, string[] properties)
        {
            ItemData itemData = database.GetItemFromID(itemID);
            if (itemData == null) { Debug.LogWarning($"Item {itemID} does not exist."); return; }

            int maxFit = itemData.weightPerUnit > 0f ? Mathf.Max(0, Mathf.FloorToInt((capacity - takenSpace) / itemData.weightPerUnit)) : quantity;
            int keepQty = Mathf.Min(quantity, maxFit); int dropQty = quantity - keepQty;

            if (keepQty > 0)
            {
                if (itemData.maxQuantityPerStack > 1)
                {
                    ItemInstance slot = GetPossibleStackSlot(itemID);
                    if (slot != null)
                    {
                        int fillAmount = Mathf.Min(keepQty, itemData.maxQuantityPerStack - slot.quantity);
                        slot.quantity += fillAmount; UpdateInventory(); if (keepQty > fillAmount) AddItem(itemID, keepQty - fillAmount, properties);
                    }
                    else
                    {
                        int addAmount = Mathf.Min(keepQty, itemData.maxQuantityPerStack);
                        storedItems.Add(new ItemInstance { ItemDataID = itemID, quantity = addAmount, customProperties = properties });
                        UpdateInventory(); if (keepQty > addAmount) AddItem(itemID, keepQty - addAmount, properties);
                    }
                }
                else
                {
                    storedItems.Add(new ItemInstance { ItemDataID = itemID, quantity = keepQty, customProperties = properties }); UpdateInventory();
                }
            }

            if (dropQty > 0) DropItem(new ItemInstance { ItemDataID = itemID, quantity = dropQty, customProperties = properties }, dropQty);
        }

        public virtual void RemoveItem(int itemIndex, int quantity) 
        { 
            if (itemIndex < 0 || itemIndex >= storedItems.Count) 
                return;
            storedItems[itemIndex].quantity -= quantity; 
            UpdateInventory(); 
        }
        public virtual void RemoveItem(ItemInstance item, int quantity) 
        { 
            item.quantity -= quantity; 
            UpdateInventory(); 
        }

        public virtual void DropItem(int itemIndex, int quantity) 
        { 
            if (itemIndex < 0 || itemIndex >= storedItems.Count) 
                return; 
            ItemInstance instance = storedItems[itemIndex]; 
            RemoveItem(itemIndex, quantity);
            SpawnDropPrefab(instance, quantity); 
        }
        public virtual void DropItem(ItemInstance item, int quantity) 
        { 
            RemoveItem(item, quantity); 
            SpawnDropPrefab(item, quantity); 
        }

        private void SpawnDropPrefab(ItemInstance item, int quantity)
        {
            ItemData data = database.GetItemFromID(item.ItemDataID);
            Vector3 dropPos = transform.position + transform.forward * dropOffset.z + transform.right * dropOffset.x + transform.up * dropOffset.y;
            ItemPickup itemDrop = Instantiate(data.dropPrefab, dropPos, Quaternion.identity);
            itemDrop.itemData = data; itemDrop.quantity = quantity; itemDrop.customProperties = item.customProperties; UpdateInventory();
        }

        public virtual void TransferItem(Inventory fromInv, Inventory toInv, int itemIndex, int quantity)
        {
            if (fromInv == toInv || itemIndex < 0 || itemIndex >= fromInv.storedItems.Count) return;
            ItemInstance item = fromInv.storedItems[itemIndex]; toInv.AddItem(item.ItemDataID, quantity, item.customProperties); fromInv.RemoveItem(itemIndex, quantity);
        }

        public virtual void UpdateInventory()
        {
            storedItems.RemoveAll(c => string.IsNullOrEmpty(c.ItemDataID) || database.GetItemFromID(c.ItemDataID) == null || c.quantity <= 0);
            takenSpace = storedItems.Sum(c => c.quantity * database.GetItemFromID(c.ItemDataID).weightPerUnit);
        }

        public ItemInstance GetPossibleStackSlot(string id) { return storedItems.FirstOrDefault(i => i.ItemDataID == id && i.quantity < database.GetItemFromID(id).maxQuantityPerStack); }
        public bool ThisFitInventory(float spaceTaken) { return (takenSpace + spaceTaken) <= capacity; }
    }

    [System.Serializable]
    public class ItemInstance 
    { 
        public string ItemDataID; 
        public int quantity; 
        public string[] customProperties; 
    }
}