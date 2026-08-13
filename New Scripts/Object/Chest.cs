using UnityEngine;

namespace TNT
{
    public class Chest : Inventory, IInteractable
    {
        public string InteractMSG { get => "Open " + chestName; set => throw new System.NotImplementedException(); }

        public string chestName;                        

        public void OnInteract(GameObject interactPoss)
        {
            Debug.Log(InteractMSG);
            InventoryUI.singleton?.OpenStorageContainer(this);
        }

        public override void UpdateInventory()
        {
            base.UpdateInventory();
            if(InventoryUI.singleton.storageContainerUI.parentInventory == this)
            {
                InventoryUI.singleton.UpdateStorageContent(this);
            }
        }

        private void OnDrawGizmos()
        {
            Vector3 dropPos = transform.position + transform.forward * dropOffset.z + transform.right * dropOffset.x + transform.up * dropOffset.y;
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(dropPos, 0.3f);
        }
    }
}
