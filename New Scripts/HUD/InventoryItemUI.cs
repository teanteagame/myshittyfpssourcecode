using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
namespace TNT
{
    public class InventoryItemUI : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerEnterHandler
    {
        public RectTransform iconHolder;
        public Image itemIcon;
        public Text itemNameText;
        public Text quantityText;
        public Image background;
        internal int inventoryItemIndex;
        internal Inventory parentInventory;
        internal InventoryUI playerUI;
        public ItemInstance thisInstance => parentInventory.storedItems[inventoryItemIndex];
        public ItemData data => parentInventory.database.GetItemFromID(thisInstance.ItemDataID);
        private Color originalColor;
        private PointerEventData dragEventData;
        private RectTransform rect;
        internal UnityEvent OnMouseHovered;
        private void Start()
        {
            originalColor = background.color;
            rect = GetComponent<RectTransform>();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            background.raycastTarget = false;
            background.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0.5f);
            playerUI.dragDummy.gameObject.SetActive(true);
            playerUI.dragIcon.sprite = itemIcon.sprite;
            playerUI.SetDropPanelActive(true);
            playerUI.quantitySelectUI.CloseBox();
            playerUI.dragDummy.position = eventData.position;
        }

        public void OnDrag(PointerEventData eventData)
        {
            dragEventData = eventData;
            Cursor.visible = true;
            playerUI.dragDummy.position = eventData.position;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            Cursor.visible = true;
            background.raycastTarget = true;
            background.color = originalColor;
            playerUI.dragDummy.gameObject.SetActive(false);
            playerUI.SetDropPanelActive(false);
            GameObject raycastObj = eventData.pointerCurrentRaycast.gameObject;
            Action<int> resolveAction = null;
            if (raycastObj == null)
            {
                resolveAction = qty => parentInventory.DropItem(thisInstance, qty);
            }
            else if (raycastObj == playerUI.playerContainerUI.dropPanel && parentInventory != playerUI.playerContainerUI.parentInventory)
            {
                Inventory target = playerUI.playerContainerUI.parentInventory;
                resolveAction = qty => parentInventory.TransferItem(parentInventory, target, inventoryItemIndex, qty);
            }
            else if (raycastObj == playerUI.storageContainerUI.dropPanel && parentInventory != playerUI.storageContainerUI.parentInventory)
            {
                Inventory target = playerUI.storageContainerUI.parentInventory;
                resolveAction = qty => parentInventory.TransferItem(parentInventory, target, inventoryItemIndex, qty);
            }
            if (resolveAction != null)
            {
                if (Input.GetKey(KeyCode.LeftControl) && data.maxQuantityPerStack > 1)
                {
                    playerUI.quantitySelectUI.gameObject.SetActive(true);
                    playerUI.quantitySelectUI.OpenQuantitySelect(parentInventory, inventoryItemIndex, resolveAction);
                }
                else
                {
                    resolveAction(thisInstance.quantity);
                }
            }
        }
        public void OnPointerEnter(PointerEventData eventData)
        {
            /* OnMouseHovered.Invoke(); */
        }
    }
}