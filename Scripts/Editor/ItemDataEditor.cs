using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ItemData))]
public class ItemDataEditor : Editor
{
    SerializedObject serialized;

    private void OnEnable()
    {
        serialized = new SerializedObject(target);
    }

    public override void OnInspectorGUI()
    {
        ItemData item = (ItemData)target;

        EditorGUILayout.LabelField("Item Properties", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("Unique ID: " + item.id, EditorStyles.miniLabel);
        item.itemName = EditorGUILayout.TextField("Item Name", item.itemName);
        SerializedProperty des = serialized.FindProperty("description");
        EditorGUILayout.PropertyField(des);
        item.icon = (Sprite)EditorGUILayout.ObjectField("Icon", item.icon, typeof(Sprite), false);
        item.itemType = EditorGUILayout.TextField("Item Type", item.itemType);

        item.weightPerUnit = EditorGUILayout.FloatField("Weight per Unit", item.weightPerUnit);
        item.maxStackSize = EditorGUILayout.IntSlider("Max Stack Size", item.maxStackSize, 1, 999);

        EditorGUILayout.Space();

        if (item.maxStackSize > 1)
        {
            EditorGUILayout.HelpBox("This item is stackable.", MessageType.Info);
        }
        else
        {
            EditorGUILayout.HelpBox("This item is not stackable.", MessageType.Warning);
        }

        // Save changes
        if (GUI.changed)
        {
            EditorUtility.SetDirty(item);
        }
    }
}
