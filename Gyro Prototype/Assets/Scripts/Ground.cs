using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Ground : MonoBehaviour
{
    [Header("Floor")]
    [SerializeField] private float floorWidth = 10f;
    [SerializeField] private float lookAheadDistance = 20f;
    [SerializeField] private float behindDistance = 2f;
    [SerializeField] private float tileSize = 1f;
    [SerializeField] private float uvScale = 1f;

    private MeshFilter _meshFilter;
    private Mesh _mesh;

    private Transform _firstPlayer, _secondPlayer;

    private void OnEnable()
    {
        _meshFilter = GetComponent<MeshFilter>();

        if (_mesh == null)
        {
            _mesh = new Mesh();
            _mesh.name = "MovingFloorMesh";
        }

        _meshFilter.sharedMesh = _mesh;
        UpdateFloor();
    }

    private void Start()
    {
        _firstPlayer = GameManager.Instance.Leader().transform;
        _secondPlayer = GameManager.Instance.Loser().transform;
    }

    private void LateUpdate()
    {
        UpdateFloor();
    }

    private void OnValidate()
    {
        floorWidth = Mathf.Max(0.1f, floorWidth);

        if (isActiveAndEnabled && _meshFilter != null)
            UpdateFloor();
    }


    private void UpdateFloor()
    {
        if (!_mesh || !_firstPlayer || !_secondPlayer)
            return;

        tileSize = Mathf.Max(0.1f, tileSize);

        // Find the rear and front boundaries of the floor.
        float backZ = Mathf.Min(_firstPlayer.position.z, _secondPlayer.position.z) - behindDistance;

        float frontZ = Mathf.Max(_firstPlayer.position.z, _secondPlayer.position.z) + lookAheadDistance;

        // Round the length up to a whole number of tiles.
        float requiredLength = frontZ - backZ;

        int tilesZ = Mathf.CeilToInt(requiredLength / tileSize);
        float floorLength = tilesZ * tileSize;

        // Use a whole number of tiles across the width too.
        int tilesX = Mathf.CeilToInt(floorWidth / tileSize);
        float actualWidth = tilesX * tileSize;

        // Position the floor at the midpoint of the required range.
        float centerZ = (backZ + frontZ) * 0.5f;

        transform.position = new Vector3(transform.position.x, transform.position.y, centerZ);

        GenerateMesh(tilesX, tilesZ, actualWidth, floorLength);
    }

    private void GenerateMesh(int tilesX, int tilesZ, float actualWidth, float floorLength)
    {
        _mesh.Clear();

        int countZ = tilesZ + 1;
        int vertexCount = (tilesX + 1) * countZ;

        Vector3[] vertices = new Vector3[vertexCount];
        Vector2[] uvs = new Vector2[vertexCount];
        int[] triangles = new int[tilesX * tilesZ * 6];

        for (int x = 0; x <= tilesX; x++)
        {
            for (int z = 0; z <= tilesZ; z++)
            {
                int index = x * countZ + z;

                // Every grid section is exactly tileSize metres.
                vertices[index] = new Vector3(-actualWidth * 0.5f + x * tileSize, 0f, -floorLength * 0.5f + z * tileSize);

                // One UV unit per metre.
                uvs[index] = new Vector2(x * tileSize * uvScale, z * tileSize * uvScale);
            }
        }

        int triIndex = 0;

        for (int x = 0; x < tilesX; x++)
        {
            for (int z = 0; z < tilesZ; z++)
            {
                int bottomLeft = x * countZ + z;
                int topLeft = bottomLeft + 1;
                int bottomRight = (x + 1) * countZ + z;
                int topRight = bottomRight + 1;

                // First triangle.
                triangles[triIndex++] = bottomLeft;
                triangles[triIndex++] = topLeft;
                triangles[triIndex++] = bottomRight;

                // Second triangle.
                triangles[triIndex++] = topLeft;
                triangles[triIndex++] = topRight;
                triangles[triIndex++] = bottomRight;
            }
        }

        _mesh.vertices = vertices;
        _mesh.triangles = triangles;
        _mesh.uv = uvs;

        _mesh.RecalculateNormals();
        _mesh.RecalculateBounds();
    }


    private void OnDisable()
    {
        if (_mesh != null)
        {
            if (Application.isPlaying)
                Destroy(_mesh);
            else
                DestroyImmediate(_mesh);
        }
    }
}
