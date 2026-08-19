using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class ReplaceSpriteMaterials : EditorWindow
{
    private Material unlitMaterial;

    [MenuItem("Tools/Sprites/Replace Lit Materials")]
    public static void ShowWindow()
    {
        GetWindow<ReplaceSpriteMaterials>("Sprite Materials");
    }

    private void OnGUI()
    {
        GUILayout.Label(
            "Replace Sprite-Lit-Default",
            EditorStyles.boldLabel
        );

        EditorGUILayout.Space();

        unlitMaterial = (Material)EditorGUILayout.ObjectField(
            "New Material",
            unlitMaterial,
            typeof(Material),
            false
        );

        EditorGUILayout.Space();

        if (GUILayout.Button("Replace Materials in Scene"))
        {
            ReplaceMaterialsInScene();
        }

        EditorGUILayout.Space();

        if (GUILayout.Button("Replace Materials in Prefabs"))
        {
            ReplaceMaterialsInPrefabs();
        }
    }

    private void ReplaceMaterialsInScene()
    {
        if (unlitMaterial == null)
        {
            Debug.LogError("Please assign Sprite-Unlit-Default first.");
            return;
        }

        SpriteRenderer[] renderers =
            FindObjectsOfType<SpriteRenderer>(true);

        int count = 0;

        foreach (SpriteRenderer renderer in renderers)
        {
            if (renderer.sharedMaterial != null &&
                renderer.sharedMaterial.name == "Sprite-Lit-Default")
            {
                Undo.RecordObject(renderer, "Replace Sprite Material");

                renderer.sharedMaterial = unlitMaterial;

                EditorUtility.SetDirty(renderer);

                count++;
            }
        }

        Debug.Log(
            "Finished! " + count +
            " SpriteRenderers were changed in the current scene."
        );
    }

    private void ReplaceMaterialsInPrefabs()
    {
        if (unlitMaterial == null)
        {
            Debug.LogError("Please assign Sprite-Unlit-Default first.");
            return;
        }

        string[] prefabGUIDs =
            AssetDatabase.FindAssets("t:Prefab");

        int changedPrefabs = 0;
        int changedRenderers = 0;

        foreach (string guid in prefabGUIDs)
        {
            string path =
                AssetDatabase.GUIDToAssetPath(guid);

            GameObject prefab =
                PrefabUtility.LoadPrefabContents(path);

            bool changed = false;

            SpriteRenderer[] renderers =
                prefab.GetComponentsInChildren<SpriteRenderer>(true);

            foreach (SpriteRenderer renderer in renderers)
            {
                if (renderer.sharedMaterial != null &&
                    renderer.sharedMaterial.name == "Sprite-Lit-Default")
                {
                    renderer.sharedMaterial = unlitMaterial;

                    changed = true;
                    changedRenderers++;
                }
            }

            if (changed)
            {
                PrefabUtility.SaveAsPrefabAsset(
                    prefab,
                    path
                );

                changedPrefabs++;
            }

            PrefabUtility.UnloadPrefabContents(prefab);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log(
            "Finished! " +
            changedPrefabs +
            " prefabs and " +
            changedRenderers +
            " SpriteRenderers were changed."
        );
    }
}
