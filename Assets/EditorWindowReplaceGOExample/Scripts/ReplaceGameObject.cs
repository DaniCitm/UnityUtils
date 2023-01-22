using UnityEditor;
using UnityEngine;

public class ReplaceGameObjectByPrefab : EditorWindow
{
    public GameObject prefab;

    // Add menu item named "My Window" to the Window menu
    [MenuItem("Tools/ReplaceGameObjectByPrefab")]
    public static void ShowWindow()
    {
        //Show existing window instance. If one doesn't exist, make one.
        EditorWindow.GetWindow(typeof(ReplaceGameObjectByPrefab));
    }

    void OnGUI()
    {
        GUILayout.Label("Replace selected GameObjects by prefab pressing the button.", EditorStyles.boldLabel);
        prefab = (GameObject)EditorGUILayout.ObjectField("Prefab", prefab, typeof(GameObject), true);

        if (GUILayout.Button("Replace Selected"))
        {
            if (!prefab) return;
            foreach (GameObject obj in Selection.gameObjects)
            {
                GameObject go = (GameObject)PrefabUtility.InstantiatePrefab(prefab, obj.transform.parent);
                Undo.RegisterCreatedObjectUndo(go, "Created go");
                if (!go) return;
                go.transform.position = obj.transform.position;
                go.transform.rotation = obj.transform.rotation;
                go.transform.SetSiblingIndex(obj.transform.GetSiblingIndex());

                Undo.DestroyObjectImmediate(obj);
            }
        }
    }
}