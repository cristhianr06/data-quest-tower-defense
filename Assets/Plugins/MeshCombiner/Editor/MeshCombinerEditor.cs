using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MeshCombiner))]
public class MeshCombinerEditor : Editor
{
    // FIX: styles cacheados, no recreados en cada repintado
    private GUIStyle _destroyToggleStyle;
    private static readonly Regex InvalidPathRegex = new Regex("[:*?\"<>|]");

    private void OnEnable()
    {
        _destroyToggleStyle = null; // Se inicializa en el primer OnInspectorGUI tras cargar estilos
    }

    public override void OnInspectorGUI()
    {
        MeshCombiner meshCombiner = (MeshCombiner)target;
        Mesh mesh = meshCombiner.GetComponent<MeshFilter>().sharedMesh;

        // Script field (read-only)
        GUI.enabled = false;
        EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour(meshCombiner),
            typeof(MeshCombiner), false);
        GUI.enabled = true;

        // MeshFilters to skip
        SerializedProperty meshFiltersToSkip = serializedObject.FindProperty("meshFiltersToSkip");
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(meshFiltersToSkip, true);
        if (EditorGUI.EndChangeCheck())
            serializedObject.ApplyModifiedProperties();

        EditorGUILayout.Space();

        // Combine button
        if (GUILayout.Button("Combine Meshes"))
        {
            Undo.RecordObject(meshCombiner, "Combine Meshes"); // FIX: soporta Undo
            meshCombiner.CombineMeshes(true);
            EditorUtility.SetDirty(meshCombiner);
        }

        EditorGUILayout.Space();

        // FIX: Undo.RecordObject antes de cada cambio de toggle
        EditorGUI.BeginChangeCheck();

        bool multiMat = GUILayout.Toggle(meshCombiner.CreateMultiMaterialMesh, "Create Multi-Material Mesh");
        bool combineInactive = GUILayout.Toggle(meshCombiner.CombineInactiveChildren, "Combine Inactive Children");
        bool deactivateChildren = GUILayout.Toggle(meshCombiner.DeactivateCombinedChildren, "Deactivate Combined Children");
        bool deactivateRenderers = GUILayout.Toggle(meshCombiner.DeactivateCombinedChildrenMeshRenderers,
            "Deactivate Combined Children's MeshRenderers");
        bool genUV = GUILayout.Toggle(meshCombiner.GenerateUVMap,
            new GUIContent("Generate UV Map",
                "Generates a UV map required for lightmaps. Slow operation. Editor only."));

        // Destroy toggle con color rojo cuando está activo
        if (_destroyToggleStyle == null)
        {
            _destroyToggleStyle = new GUIStyle(EditorStyles.toggle);
            _destroyToggleStyle.onNormal.textColor = new Color(1f, 0.15f, 0f);
        }
        bool destroyChildren = GUILayout.Toggle(meshCombiner.DestroyCombinedChildren,
            new GUIContent("Destroy Combined Children",
                "Cannot be undone in Editor! Reload the scene without saving to recover."),
            _destroyToggleStyle);

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(meshCombiner, "MeshCombiner Settings"); // FIX
            meshCombiner.CreateMultiMaterialMesh = multiMat;
            meshCombiner.CombineInactiveChildren = combineInactive;
            meshCombiner.DeactivateCombinedChildren = deactivateChildren;
            meshCombiner.DeactivateCombinedChildrenMeshRenderers = deactivateRenderers;
            meshCombiner.GenerateUVMap = genUV;
            meshCombiner.DestroyCombinedChildren = destroyChildren;
            EditorUtility.SetDirty(meshCombiner);
        }

        // Folder path
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Folder path:", EditorStyles.boldLabel);

        bool isValidPath = IsValidPath(meshCombiner.FolderPath);
        GUIStyle pathStyle = new GUIStyle(EditorStyles.textField);
        if (!isValidPath)
        {
            pathStyle.normal.textColor = Color.red;
            pathStyle.focused.textColor = Color.red;
        }

        EditorGUI.BeginChangeCheck();
        string newPath = EditorGUILayout.TextField(meshCombiner.FolderPath, pathStyle);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(meshCombiner, "MeshCombiner FolderPath");
            meshCombiner.FolderPath = newPath;
            EditorUtility.SetDirty(meshCombiner);
        }

        // Save / show mesh button
        bool meshIsSaved = mesh != null && AssetDatabase.Contains(mesh);
        GUI.enabled = mesh != null && (isValidPath || meshIsSaved);
        string saveLabel = meshIsSaved ? "Show Saved Combined Mesh" : "Save Combined Mesh";

        if (GUILayout.Button(saveLabel))
            meshCombiner.FolderPath = SaveCombinedMesh(mesh, meshCombiner.FolderPath);

        GUI.enabled = true;
    }

    private static bool IsValidPath(string path) => !InvalidPathRegex.IsMatch(path);

    private static string SaveCombinedMesh(Mesh mesh, string folderPath)
    {
        if (AssetDatabase.Contains(mesh))
        {
            EditorGUIUtility.PingObject(mesh);
            return folderPath;
        }

        folderPath = folderPath.Replace('\\', '/');

        if (!AssetDatabase.IsValidFolder("Assets/" + folderPath))
        {
            string[] parts = folderPath.Split('/')
                .Where(p => !string.IsNullOrWhiteSpace(p))
                .Select(p => p.Trim())
                .ToArray();

            string current = string.Empty;
            foreach (var part in parts)
            {
                string next = string.IsNullOrEmpty(current) ? part : $"{current}/{part}";
                if (!AssetDatabase.IsValidFolder("Assets/" + next))
                    AssetDatabase.CreateFolder("Assets/" + current, part);
                current = next;
            }
            folderPath = current;
        }

        string basePath = $"Assets/{folderPath}/{mesh.name}";
        string meshPath = $"{basePath}.asset";
        int n = 1;
        while (AssetDatabase.LoadAssetAtPath<Mesh>(meshPath) != null)
            meshPath = $"{basePath} ({n++}).asset";

        AssetDatabase.CreateAsset(mesh, meshPath);
        AssetDatabase.SaveAssets();
        Debug.Log($"<color=#ff9900><b>Mesh \"{mesh.name}\" saved at \"{folderPath}\".</b></color>");
        EditorGUIUtility.PingObject(mesh);
        return folderPath;
    }
}