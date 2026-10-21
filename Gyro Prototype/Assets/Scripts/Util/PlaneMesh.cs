using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
[ExecuteAlways]
public class PlaneMesh : MonoBehaviour
{
    [Header("Strip Settings")]
    [SerializeField] private Vector2 size = new Vector2(10f, 1f);
    [SerializeField] private float uvScale = 1f;
    [SerializeField] private Vector2Int subdivisions = new Vector2Int(10, 1);

    private Mesh _mesh;

    private void OnEnable()
    {
        GenerateStrip();
    }

    private void OnValidate()
    {
        if (isActiveAndEnabled) GenerateStrip();
    }

    private void GenerateStrip()
    {
        // Setup
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        _mesh = meshFilter.sharedMesh;
        if (_mesh == null)
        {
            _mesh = new Mesh();
            _mesh.name = "FloorStrip";
        }
        meshFilter.sharedMesh = _mesh;
        _mesh.Clear();

        // Variables
        List<Vector3> vertices = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> triangles = new List<int>();

        // Make vertices
        Vector2 start = -size * 0.5f;
        float stepX = size.x / subdivisions.x;
        float stepZ = size.y / subdivisions.y;

        for (int z = 0; z <= subdivisions.y; z++)
        {
            for (int x = 0; x <= subdivisions.x; x++)
            {
                float xPos = start.x + (x * stepX);
                float zPos = start.y + (z * stepZ);
                vertices.Add(new Vector3(xPos, 0f, zPos));

                // UV mapping normalized 0 to 1, scaled by uvScale
                float u = ((float)x / subdivisions.x) * uvScale;
                float v = ((float)z / subdivisions.y) * uvScale;
                uvs.Add(new Vector2(u * size.x, v * size.y));
            }
        }

        // Make triangles
        int verticesPerRow = subdivisions.x + 1;
        
        for (int z = 0; z < subdivisions.y; z++)
        {
            for (int x = 0; x < subdivisions.x; x++)
            {
                int row1 = z * verticesPerRow;
                int row2 = (z + 1) * verticesPerRow;

                triangles.Add(row2 + x);
                triangles.Add(row1 + x + 1);
                triangles.Add(row1 + x);

                triangles.Add(row2 + x);
                triangles.Add(row2 + x + 1);
                triangles.Add(row1 + x + 1);
            }
        }

        _mesh.vertices = vertices.ToArray();
        _mesh.uv = uvs.ToArray();
        _mesh.triangles = triangles.ToArray();

        _mesh.RecalculateNormals();
        _mesh.RecalculateBounds();
    }
}
