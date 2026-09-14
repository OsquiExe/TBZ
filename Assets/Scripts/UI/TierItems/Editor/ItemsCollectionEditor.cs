using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ItemsCollection))]
public class ItemsCollectionEditor : Editor
{
    ItemsCollection t;

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        if(t == null) {
            t = (ItemsCollection)target;
        }

        if(t.IsEmpty() && GUILayout.Button("Get From Assets")) {
            t.Populate(GetAllInstances<EquipmentItemSO>().ToArray());
            EditorUtility.SetDirty(t);
        }
    }

    public static List<T> GetAllInstances<T>() where T : ScriptableObject {
        List<T> results = new List<T>();

        // Search the AssetDatabase using the filter "t:TypeName"
        string[] guids = AssetDatabase.FindAssets($"t:{typeof(T).Name}");

        foreach(string guid in guids) {
            // Convert the unique GUID to a regular asset path
            string path = AssetDatabase.GUIDToAssetPath(guid);

            // Load the asset from the path and cast it
            T asset = AssetDatabase.LoadAssetAtPath<T>(path);

            if(asset != null) {
                results.Add(asset);
            }
        }

        return results;
    }
}
