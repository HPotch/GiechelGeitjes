using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Spawner : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Vector2 spawnDistance = new Vector2(0.3f, 4f);
    [SerializeField] private bool outside = false;
    [SerializeField] private float maxOutside = 20f, outsideOffset = 7f;
    [SerializeField] private bool dontChangeRotation = false;

    [SerializeField] private float spawnDistanceToLeader = 20f;
    [Header("References")]
    [SerializeField] private List<GameObject> prefabs = new List<GameObject>();

    private float _lastLeaderZ;
    private bool _spawning = false;

    private void Update()
    {
        if (_spawning) return;
        _spawning = true;
        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        float distance = Random.Range(spawnDistance.x, spawnDistance.y);
        float startZ = GameManager.Instance.Leader().position.z;
        
        yield return new WaitUntil(() => Mathf.Abs(startZ - GameManager.Instance.Leader().position.z) > distance);
        
        _spawning = false;
        SpawnObstacle();
    }

    private void SpawnObstacle()
    {
        if (prefabs.Count == 0) return;

        Transform leader = GameManager.Instance.Leader();

        GameObject obstacle;
        if (dontChangeRotation)
            obstacle = Instantiate(prefabs[Random.Range(0, prefabs.Count)], transform);
        else
            obstacle = Instantiate(prefabs[Random.Range(0, prefabs.Count)], transform.position, transform.rotation * Quaternion.Euler(0f, 0f, 0f), transform);
        //obstacle.transform.rotation = Quaternion.identity;

        float bounds = GameManager.Instance.bounds;
        if (outside)
        {
            bounds += outsideOffset;
            Vector2 range = Random.Range(0,2) == 0
                ? new Vector2(-bounds - maxOutside, -bounds) // Left
                : new Vector2(bounds, bounds + maxOutside); // Right
            obstacle.transform.localPosition = new Vector3(Random.Range(range.x, range.y), transform.position.y, leader.position.z + spawnDistanceToLeader);
        }
        else
        {
            obstacle.transform.localPosition = new Vector3(Random.Range(-bounds, bounds), transform.position.y, leader.position.z + spawnDistanceToLeader);
        }
        
    }
}
