
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class PlaneMesh : MonoBehaviour
{
    [Header("Strip Settings")]
    [SerializeField] private float floorWidth = 10f;
    [SerializeField] private float tileSize = 1f;
    [SerializeField] private float uvScale = 1f;

    private Mesh mesh;

    private void OnEnable()
    {
        GenerateStrip();
    }

    private void OnValidate()
    {
        floorWidth = Mathf.Max(0.1f, floorWidth);
        tileSize = Mathf.Max(0.1f, tileSize);

        if (isActiveAndEnabled)
            GenerateStrip();
    }

    private void GenerateStrip()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();

        if (mesh == null)
        {
            mesh = new Mesh();
            mesh.name = "FloorStrip";
        }

        meshFilter.sharedMesh = mesh;
        mesh.Clear();

        float halfWidth = floorWidth * 0.5f;

        Vector3[] vertices =
        {
            new Vector3(-halfWidth, 0f, 0f),
            new Vector3( halfWidth, 0f, 0f),
            new Vector3(-halfWidth, 0f, tileSize),
            new Vector3( halfWidth, 0f, tileSize)
        };

        Vector2[] uvs =
        {
            new Vector2(0f, 0f),
            new Vector2(floorWidth * uvScale, 0f),
            new Vector2(0f, tileSize * uvScale),
            new Vector2(floorWidth * uvScale, tileSize * uvScale)
        };

        int[] triangles =
        {
            0, 2, 1,
            2, 3, 1
        };

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }

    private void OnDestroy()
    {
        if (mesh != null)
        {
            if (Application.isPlaying)
                Destroy(mesh);
            else
                DestroyImmediate(mesh);
        }
    }
}
