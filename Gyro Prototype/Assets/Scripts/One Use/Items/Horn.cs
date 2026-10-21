using UnityEngine;

public class Horn : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject destroyerGoatPrefab;
    [SerializeField] private int goatsOnOneSide = 4;
    [SerializeField] private float spawnDistance = 1f;
    
    private RoadObject _roadObject;
    private Transform _parent;

    private void Awake()
    {
        _roadObject = GetComponent<RoadObject>();
        if (!_roadObject)
            Debug.LogWarning("Item doesn't have a road object attached");
        else
            _roadObject.onExecute.AddListener(Execute);
        
    }

    private void Execute()
    {
        for (int i = 0; i < goatsOnOneSide; i++)
        {
            SpawnGoat((i + 1) * -spawnDistance); // Spawn one left
            SpawnGoat((i + 1) * spawnDistance); // Spawn one right
        }
    }

    private void SpawnGoat(float x)
    {
        GameObject goat = Instantiate(destroyerGoatPrefab, transform.root);
        Vector3 playerPos = _roadObject.player.transform.position;
        goat.transform.position = new Vector3(playerPos.x + x, playerPos.y, playerPos.z);
        DestroyerGoat destroyerGoat = goat.GetComponent<DestroyerGoat>();
        destroyerGoat.player = _roadObject.player.transform;
        destroyerGoat.StartRun();
    }
}