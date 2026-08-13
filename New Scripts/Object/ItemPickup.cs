using UnityEngine;

namespace TNT
{
    public class ItemPickup : MonoBehaviour, IInteractable
    {
        public ItemData itemData;
        public int quantity;
        public string[] customProperties;

        public string InteractMSG { get => "Pick up " + itemData != null ? itemData.itemName : "this item"; set => throw new System.NotImplementedException(); }

        public void OnInteract(GameObject interactPoss)
        {
            if(interactPoss.TryGetComponent(out PlayerInventory inventory))
            {
                inventory.AddItem(itemData.ItemID, quantity, customProperties);
                Destroy(gameObject);
            }
        }
    }
}
