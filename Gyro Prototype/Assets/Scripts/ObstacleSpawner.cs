using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ObstacleSpawner : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Vector2 spawnDistance = new Vector2(0.3f, 4f);
    [SerializeField] private float distanceAhead = 20f;

    [Header("References")]
    [SerializeField] private List<GameObject> obstaclePrefabs = new List<GameObject>();


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

        while (distance > 0f)
        {
            distance -= GameManager.Instance.moveAmount * Time.deltaTime;
            yield return null;
        }
        
        _spawning = false;
        SpawnObstacle();
    }

    private void SpawnObstacle()
    {
        Transform leader = GameManager.Instance.Leader();

        Vector3 spawnPosition = leader.position + Vector3.forward * distanceAhead;

		GameObject obstacle = Instantiate(obstaclePrefabs[Random.Range(0, obstaclePrefabs.Count)], transform);
        float range = GameManager.Instance.bounds;
        obstacle.transform.localPosition = new Vector3(0f, 0f, Random.Range(-range, range));
    }
}
