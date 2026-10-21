using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Spawner : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Vector2 spawnDistance = new Vector2(0.3f, 4f);

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
        float range = GameManager.Instance.bounds;

        GameObject obstacle = Instantiate(prefabs[Random.Range(0, prefabs.Count)], transform.position, transform.rotation * Quaternion.Euler(0f, 0f, 0f), transform);
        //obstacle.transform.rotation = Quaternion.identity;
        
        obstacle.transform.localPosition = new Vector3(Random.Range(-range, range), transform.position.y, leader.position.z + spawnDistanceToLeader);
    }
}
