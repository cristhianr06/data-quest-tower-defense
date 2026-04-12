using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class MeshCombiner : MonoBehaviour
{
    private const int Mesh16BitBufferVertexLimit = 65_535;
    private const int QuestPerformanceWarningLimit = 100_000;

    [SerializeField] private bool createMultiMaterialMesh = false;
    [SerializeField] private bool combineInactiveChildren = false;
    [SerializeField] private bool deactivateCombinedChildren = true;
    [SerializeField] private bool deactivateCombinedChildrenMeshRenderers = false;
    [SerializeField] private bool generateUVMap = false;
    [SerializeField] private bool destroyCombinedChildren = false;
    [SerializeField] private string folderPath = "_project/Art/Models/CombinedMeshes";

    [SerializeField]
    [Tooltip("MeshFilters with Meshes which we don't want to combine into one Mesh.")]
    private MeshFilter[] meshFiltersToSkip = new MeshFilter[0];

    #region Properties
    public bool CreateMultiMaterialMesh
    {
        get => createMultiMaterialMesh;
        set => createMultiMaterialMesh = value;
    }
    public bool CombineInactiveChildren
    {
        get => combineInactiveChildren;
        set => combineInactiveChildren = value;
    }
    public bool DeactivateCombinedChildren
    {
        get => deactivateCombinedChildren;
        set { deactivateCombinedChildren = value; CheckDeactivateCombinedChildren(); }
    }
    public bool DeactivateCombinedChildrenMeshRenderers
    {
        get => deactivateCombinedChildrenMeshRenderers;
        set { deactivateCombinedChildrenMeshRenderers = value; CheckDeactivateCombinedChildren(); }
    }
    public bool GenerateUVMap
    {
        get => generateUVMap;
        set => generateUVMap = value;
    }
    public bool DestroyCombinedChildren
    {
        get => destroyCombinedChildren;
        set { destroyCombinedChildren = value; CheckDestroyCombinedChildren(); }
    }
    public string FolderPath
    {
        get => folderPath;
        set => folderPath = value;
    }
    #endregion

    private void CheckDeactivateCombinedChildren()
    {
        if (deactivateCombinedChildren || deactivateCombinedChildrenMeshRenderers)
            destroyCombinedChildren = false;
    }

    private void CheckDestroyCombinedChildren()
    {
        if (destroyCombinedChildren)
        {
            deactivateCombinedChildren = false;
            deactivateCombinedChildrenMeshRenderers = false;
        }
    }

    public void CombineMeshes(bool showCreatedMeshInfo)
    {
        Vector3 oldScaleAsChild = transform.localScale;
        int siblingIndex = transform.GetSiblingIndex();
        Transform parent = transform.parent;
        transform.parent = null;

        Quaternion oldRotation = transform.rotation;
        Vector3 oldPosition = transform.position;
        Vector3 oldScale = transform.localScale;
        transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
        transform.localScale = Vector3.one;

        if (!createMultiMaterialMesh)
            CombineMeshesWithSingleMaterial(showCreatedMeshInfo);
        else
            CombineMeshesWithMutliMaterial(showCreatedMeshInfo);

        transform.SetPositionAndRotation(oldPosition, oldRotation);
        transform.localScale = oldScale;
        transform.parent = parent;
        transform.SetSiblingIndex(siblingIndex);
        transform.localScale = oldScaleAsChild;
    }

    private MeshFilter[] GetMeshFiltersToCombine()
    {
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>(combineInactiveChildren);
        HashSet<MeshFilter> skipSet = new HashSet<MeshFilter>(meshFiltersToSkip.Where(mf => mf != null));
        skipSet.Remove(meshFilters[0]);
        meshFiltersToSkip = skipSet.ToArray();
        return meshFilters.Where((mf, i) => i == 0 || !skipSet.Contains(mf)).ToArray();
    }

    private void CombineMeshesWithSingleMaterial(bool showCreatedMeshInfo)
    {
        MeshFilter[] meshFilters = GetMeshFiltersToCombine();
        CombineInstance[] combineInstances = new CombineInstance[meshFilters.Length - 1];

        long verticesLength = 0;
        for (int i = 0; i < meshFilters.Length - 1; i++)
        {
            combineInstances[i].subMeshIndex = 0;
            combineInstances[i].mesh = meshFilters[i + 1].sharedMesh;
            combineInstances[i].transform = meshFilters[i + 1].transform.localToWorldMatrix;
            verticesLength += meshFilters[i + 1].sharedMesh.vertexCount;
        }

        MeshRenderer[] meshRenderers = GetComponentsInChildren<MeshRenderer>(combineInactiveChildren);
        if (meshRenderers.Length >= 2)
        {
            meshRenderers[0].sharedMaterials = new Material[1];
            meshRenderers[0].sharedMaterial = meshRenderers[1].sharedMaterial;
        }
        else
        {
            meshRenderers[0].sharedMaterials = new Material[0];
        }

        Mesh combinedMesh = new Mesh { name = name };
        combinedMesh.indexFormat = ResolveIndexFormat(verticesLength);
        combinedMesh.CombineMeshes(combineInstances);
        GenerateUV(combinedMesh);

        Mesh previousMesh = meshFilters[0].sharedMesh;
        meshFilters[0].sharedMesh = combinedMesh;
        DestroyPreviousMeshIfUnused(previousMesh); // FIX

        DeactivateCombinedGameObjects(meshFilters);
        LogMeshInfo(showCreatedMeshInfo, meshFilters.Length - 1, 1, verticesLength);
    }

    private void CombineMeshesWithMutliMaterial(bool showCreatedMeshInfo)
    {
        MeshFilter[] meshFilters = GetMeshFiltersToCombine();
        MeshRenderer[] meshRenderers = new MeshRenderer[meshFilters.Length];
        meshRenderers[0] = GetComponent<MeshRenderer>();

        List<Material> uniqueMaterials = new List<Material>();
        for (int i = 0; i < meshFilters.Length - 1; i++)
        {
            meshRenderers[i + 1] = meshFilters[i + 1].GetComponent<MeshRenderer>();
            if (meshRenderers[i + 1] == null) continue;

            foreach (var mat in meshRenderers[i + 1].sharedMaterials)
            {
                if (mat != null && !uniqueMaterials.Contains(mat))
                    uniqueMaterials.Add(mat);
            }
        }

        HashSet<Mesh> countedMeshes = new HashSet<Mesh>();
        long verticesLength = 0;
        for (int i = 0; i < meshFilters.Length - 1; i++)
        {
            Mesh m = meshFilters[i + 1].sharedMesh;
            if (m != null && countedMeshes.Add(m))
                verticesLength += m.vertexCount;
        }

        UnityEngine.Rendering.IndexFormat indexFormat = ResolveIndexFormat(verticesLength);

        List<CombineInstance> finalCombineInstances = new List<CombineInstance>();
        List<Mesh> tempSubmeshes = new List<Mesh>();

        for (int i = 0; i < uniqueMaterials.Count; i++)
        {
            List<CombineInstance> submeshInstances = new List<CombineInstance>();

            for (int j = 0; j < meshFilters.Length - 1; j++)
            {
                if (meshRenderers[j + 1] == null) continue;

                Material[] childMaterials = meshRenderers[j + 1].sharedMaterials;
                Mesh childMesh = meshFilters[j + 1].sharedMesh;

                for (int k = 0; k < childMaterials.Length; k++)
                {
                    if (uniqueMaterials[i] != childMaterials[k]) continue;

                    submeshInstances.Add(new CombineInstance
                    {
                        subMeshIndex = k,
                        mesh = childMesh,
                        transform = meshFilters[j + 1].transform.localToWorldMatrix
                    });
                }
            }

            Mesh submesh = new Mesh { indexFormat = indexFormat };
            submesh.CombineMeshes(submeshInstances.ToArray(), true);
            tempSubmeshes.Add(submesh);

            finalCombineInstances.Add(new CombineInstance
            {
                subMeshIndex = 0,
                mesh = submesh,
                transform = Matrix4x4.identity
            });
        }

        meshRenderers[0].sharedMaterials = uniqueMaterials.ToArray();

        Mesh combinedMesh = new Mesh { name = name, indexFormat = indexFormat };
        combinedMesh.CombineMeshes(finalCombineInstances.ToArray(), false);
        GenerateUV(combinedMesh);

        Mesh previousMesh = meshFilters[0].sharedMesh;
        meshFilters[0].sharedMesh = combinedMesh;
        DestroyPreviousMeshIfUnused(previousMesh); // FIX

        foreach (var submesh in tempSubmeshes)
            DestroyImmediate(submesh);

        DeactivateCombinedGameObjects(meshFilters);
        LogMeshInfo(showCreatedMeshInfo, meshFilters.Length - 1, finalCombineInstances.Count, verticesLength);
    }

    private static UnityEngine.Rendering.IndexFormat ResolveIndexFormat(long vertexCount)
    {
        return vertexCount > Mesh16BitBufferVertexLimit
            ? UnityEngine.Rendering.IndexFormat.UInt32
            : UnityEngine.Rendering.IndexFormat.UInt16;
    }

    /// <summary>
    /// Destruye el mesh anterior solo si no está guardado como asset.
    /// Usa #if UNITY_EDITOR para evitar referencias a UnityEditor en builds.
    /// </summary>
    private static void DestroyPreviousMeshIfUnused(Mesh previousMesh)
    {
        if (previousMesh == null) return;
#if UNITY_EDITOR
        if (!UnityEditor.AssetDatabase.Contains(previousMesh))
            DestroyImmediate(previousMesh);
#else
        Destroy(previousMesh);
#endif
    }

    private void DeactivateCombinedGameObjects(MeshFilter[] meshFilters)
    {
        for (int i = 1; i < meshFilters.Length; i++)
        {
            if (destroyCombinedChildren)
            {
#if UNITY_EDITOR
                UnityEditor.Undo.DestroyObjectImmediate(meshFilters[i].gameObject);
#else
                Destroy(meshFilters[i].gameObject);
#endif
            }
            else
            {
                if (deactivateCombinedChildren)
                    meshFilters[i].gameObject.SetActive(false);

                if (deactivateCombinedChildrenMeshRenderers)
                {
                    var mr = meshFilters[i].GetComponent<MeshRenderer>();
                    if (mr != null) mr.enabled = false;
                }
            }
        }
    }

    private void GenerateUV(Mesh combinedMesh)
    {
#if UNITY_EDITOR
        if (!generateUVMap) return;
        UnityEditor.UnwrapParam.SetDefaults(out var unwrapParam);
        UnityEditor.Unwrapping.GenerateSecondaryUVSet(combinedMesh, unwrapParam);
#endif
    }

    private void LogMeshInfo(bool show, int childCount, int submeshCount, long vertexCount)
    {
        if (!show) return;

        string indexFormat = vertexCount > Mesh16BitBufferVertexLimit ? "UInt32" : "UInt16";
        string submeshInfo = submeshCount > 1 ? $", {submeshCount} submeshes" : string.Empty;
        string baseInfo = $"[MeshCombiner] \"{name}\" — {childCount} meshes{submeshInfo}, {vertexCount:N0} vertices ({indexFormat})";

        if (vertexCount <= Mesh16BitBufferVertexLimit)
        {
            Debug.Log($"<color=#00cc00><b>{baseInfo}.</b></color>");
        }
        else if (vertexCount <= QuestPerformanceWarningLimit)
        {
            Debug.Log($"<color=#ff9900><b>{baseInfo}. " +
                      $"Over UInt16 limit — UInt32 applied. Within Quest performance budget.</b></color>");
        }
        else
        {
            Debug.LogWarning($"<color=#ff3300><b>{baseInfo}. " +
                             $"Exceeds {QuestPerformanceWarningLimit:N0} vertex Quest 2 budget. " +
                             $"Consider reducing geometry or splitting this mesh.</b></color>");
        }
    }
}