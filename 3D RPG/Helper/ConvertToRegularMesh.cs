using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConvertToRegularMesh : MonoBehaviour
{
    [ContextMenu("Convert to Regular Mesh")]
    void Convert()
    {
        SkinnedMeshRenderer skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
        MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();

        meshFilter.sharedMesh = skinnedMeshRenderer.sharedMesh;
        meshRenderer.sharedMaterials = skinnedMeshRenderer.sharedMaterials;

        DestroyImmediate(skinnedMeshRenderer);
        DestroyImmediate(this);
    }
}
