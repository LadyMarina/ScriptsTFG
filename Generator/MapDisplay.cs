using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDisplay : MonoBehaviour
{

    [Header("MESH OBJECT")]
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;
    public GameObject meshObject;

    [Header("TEXTURE OBJECT")]
    public Renderer textureRenderer;
    public GameObject displayObject;

    public void DrawTexture(Texture2D texture)
    {
        textureRenderer.sharedMaterial.mainTexture = texture;
        textureRenderer.transform.localScale = new Vector3(texture.width, 1, texture.height);
    }

    public void DrawMesh(MeshData meshData, Texture2D texture)
    {
        meshFilter.sharedMesh = meshData.CreateMesh();
        meshRenderer.sharedMaterial.mainTexture = texture;
    }
}
