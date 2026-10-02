using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class PlaneMesh : MonoBehaviour
{
    [SerializeField] [Range(1, 100)] private int subdivisions = 10;
    [SerializeField] private float size = 10f;
    [SerializeField] private float uvScale = 1f;

    private MeshFilter _meshFilter;
    private Mesh _mesh;

    private void OnEnable()
    {
        _meshFilter = GetComponent<MeshFilter>();
        
        if (_mesh == null)
        {
            _mesh = new Mesh();
            _mesh.name = "PlaneMesh";
            _meshFilter.sharedMesh = _mesh;
        }
        
        GenerateMesh();
    }

    // Regenerate the mesh when inspector values change
    private void OnValidate()
    {
        if (_meshFilter != null)
        {
            GenerateMesh();
        }
    }

    private void GenerateMesh()
    {
        if (_mesh == null) return;
        
        // Clear
        _mesh.Clear();

        // Setup variables
        int verticesCount = (subdivisions + 1) * (subdivisions + 1);
        Vector3[] vertices = new Vector3[verticesCount];
        Vector2[] uvs = new Vector2[verticesCount];
        int[] triangles = new int[subdivisions * subdivisions * 6];

        float stepSize = size / subdivisions;
        Vector3 offset = new Vector3(size, 0, size) * 0.5f;

        // Generate vertices and UVs
        for (int i = 0; i <= subdivisions; i++)
        {
            for (int j = 0; j <= subdivisions; j++)
            {
                int index = i * (subdivisions + 1) + j;
                
                vertices[index] = new Vector3(i * stepSize, 0, j * stepSize) - offset;
                
                // Repeat UVs
                float u = (float)i / subdivisions;
                float v = (float)j / subdivisions;
                uvs[index] = new Vector2(u * uvScale, v * uvScale);
            }
        }

        // Generate triangles
        int triIndex = 0;
        for (int i = 0; i < subdivisions; i++)
        {
            for (int j = 0; j < subdivisions; j++)
            {
                int current = i * (subdivisions + 1) + j;
                int nextRow = (i + 1) * (subdivisions + 1) + j;
                
                triangles[triIndex] = current;
                triangles[triIndex + 1] = current + 1;
                triangles[triIndex + 2] = nextRow;
                triIndex += 3;

                triangles[triIndex] = current + 1;
                triangles[triIndex + 1] = nextRow + 1;
                triangles[triIndex + 2] = nextRow;
                triIndex += 3;
            }
        }

        // Apply
        _mesh.vertices = vertices;
        _mesh.triangles = triangles;
        _mesh.uv = uvs;
        _mesh.RecalculateNormals();
        _mesh.RecalculateBounds();
    }
}