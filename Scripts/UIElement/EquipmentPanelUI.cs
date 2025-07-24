using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EquipmentPanelUI : MonoBehaviour, IPointerClickHandler
{
    public ItemData equipedItem;
    public string allowedItemType;

    [Header("UI")]
    public RectTransform iconHolder;
    public Image icon;
    public Sprite defaultIcon;

    [Header("Attachment")]
    public bool useAttachment;
    public GameObject sightSlot;
    public GameObject magSlot;
    public GameObject gripSlot;
    public GameObject muzzleSlot;
    public GameObject stockSlot;

    private void Update()
    {
        if (equipedItem != null) 
        {

        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        
    }  
}
