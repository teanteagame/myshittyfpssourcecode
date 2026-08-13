using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TNT
{
    public class Database : MonoBehaviour
    {
        public static Database singleton;
        public List<ItemData> items;
        private Dictionary<string, ItemData> nameDict;
        private Dictionary<string, ItemData> idDict;

        private void Awake() { singleton = this; InitializeDictionaries(); }
        private void InitializeDictionaries() { nameDict = items.ToDictionary(i => i.name, i => i); idDict = items.ToDictionary(i => i.ItemID, i => i); }
        public ItemData GetItemFromName(string name) { return nameDict != null && nameDict.TryGetValue(name, out var item) ? item : null; }
        public ItemData GetItemFromID(string ID) { return idDict != null && idDict.TryGetValue(ID, out var item) ? item : null; }
        public ItemData GetItemFromIndex(int index) { return index >= 0 && index < items.Count ? items[index] : null; }
    }
}