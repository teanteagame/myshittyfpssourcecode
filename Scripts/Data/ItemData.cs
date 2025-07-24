using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    public string id;
    public string itemName;
    [TextArea]
    public string description;
    public Sprite icon;
    public string itemType;
    public float weightPerUnit;
    public int maxStackSize = 1;

    public bool isUsable;
    public bool isEquippable;

    private void OnValidate()
    {
        if (string.IsNullOrEmpty(id))
        {
            id = System.Guid.NewGuid().ToString();
        }
    }
}
