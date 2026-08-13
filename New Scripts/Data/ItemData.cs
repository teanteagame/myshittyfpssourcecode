using System;
using UnityEngine;

namespace TNT
{
    [CreateAssetMenu(menuName = "Data/New Item")]
    public class ItemData : ScriptableObject
    {
        [Header("Info")]
        public string ItemID;
        public string itemName;
        [TextArea] public string itemDecs;
        public Sprite itemIcon;

        [Header("Settings")]
        [Range(1, 999)] public int maxQuantityPerStack = 1;
        public float weightPerUnit = 1;

        public ItemPickup dropPrefab;

        public virtual void OnValidate()
        {
            if (string.IsNullOrEmpty(ItemID)) ItemID = Guid.NewGuid().ToString();
            if (dropPrefab)
                if (dropPrefab.itemData == null) dropPrefab.itemData = this;
        }
    }
}
