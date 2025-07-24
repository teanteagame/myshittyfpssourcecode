using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class InventoryItemUI : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerClickHandler
{
    public RectTransform iconHolder;
    public Image itemIcon;
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI quantityText;
    public Image background;

    internal int inventoryItemIndex;
    internal InventoryObject parentInventory;
    internal RectTransform parentTransform;
    internal PlayerUI playerUI;

    public bool drag;
    private Color originalColor;
    private PointerEventData dragEventData;
    private RectTransform rect;

    private void Start()
    {
        originalColor = background.color;
        rect = GetComponent<RectTransform>();
    }

    private void Update()
    {
        if (drag)
        {
            rect.pivot = new Vector2(Mathf.Lerp(rect.pivot.x, 0.5f, Time.deltaTime * 30), Mathf.Lerp(rect.pivot.y, 0.5f, Time.deltaTime * 30));
            rect.position = Vector2.Lerp(rect.position, dragEventData.position, Time.deltaTime * 30);
        }
        else
        {
            rect.pivot = new Vector2(Mathf.Lerp(rect.pivot.x, 0f, Time.deltaTime * 10), Mathf.Lerp(rect.pivot.y, 1f, Time.deltaTime * 30));
            rect.anchoredPosition = Vector2.Lerp(rect.anchoredPosition, Vector2.zero, Time.deltaTime * 30);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
       
    } 

    public void OnBeginDrag(PointerEventData eventData)
    {
        transform.SetParent(playerUI.onDragLayer, true);
        transform.SetAsLastSibling();
        background.raycastTarget = false;
        background.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0.5f);
        playerUI.EnableInventoryPanel(true);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Cursor.visible = false;
        dragEventData = eventData;
        drag = true;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        var eventDataRaycast = eventData.pointerCurrentRaycast;
        Cursor.visible = true;
        drag = false;
        background.color = originalColor;
        background.raycastTarget = true;


        if (eventDataRaycast.gameObject == null)
        {
            ///Nothing so drop it
            ///parentInventory.DropItem(thisInstance, thisInstance.quantity);
        }
        else
        {
            if (eventDataRaycast.gameObject.GetComponent<InventoryPanelUI>())
            {
                InventoryPanelUI inventoryUI = eventDataRaycast.gameObject.GetComponent<InventoryPanelUI>();
                ///Transfer to another inventory
                if (inventoryUI.parentInventory != parentInventory)
                {
                    TransferToInventory(thisInstance, inventoryUI.parentInventory);
                }
            }
            else if (eventDataRaycast.gameObject.GetComponent<EquipmentPanelUI>()) 
            {
                EquipmentPanelUI equipmentUI = eventDataRaycast.gameObject.GetComponent<EquipmentPanelUI>();
                ///Equip this
                if (equipmentUI.allowedItemType == thisInstance.data.itemType)
                {
                    if (equipmentUI.equipedItem != null)
                    {
                        //Replace the item
                    }
                    else
                    {
                        //Equip this item
                        equipmentUI.equipedItem = thisInstance.data;
                        parentInventory.RemoveItem(thisInstance, 1);
                    }
                }
            }
        }
        ReturnTheItem();
        playerUI.EnableInventoryPanel(false);
    }

    public void ReturnTheItem()
    {
        ///Return to original place
        transform.SetParent(parentTransform, true);
        transform.SetAsLastSibling();
        parentInventory.UpdateInventory();
    }

    public void TransferToInventory(ItemInstance item, InventoryObject toInv) 
    {
        if(item == null || toInv == null) return;

        toInv.AddItem(item.data, item.quantity, item.properties);
        parentInventory.RemoveItem(item, item.quantity);
    }

    public ItemInstance thisInstance
    {
        get
        {
            return parentInventory.storedItems[inventoryItemIndex];
        }
    }
}
