using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ObstacleSpawner : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Vector2 spawnTime = new Vector2(2f, 5f);
    [Header("References")]
    [SerializeField] private List<GameObject> obstaclePrefabs = new List<GameObject>();

    private BoxCollider2D _bounds;

    private bool _spawning = false;

    private void Awake()
    {
        _bounds = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        if (_spawning) return;
        _spawning = true;
        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        float timer = Random.Range(spawnTime.x, spawnTime.y);

        while (timer > 0f)
        {
            timer -= Time.deltaTime;
            yield return null;
        }
        
        _spawning = false;
        SpawnObstacle();
    }

    private void SpawnObstacle()
    {
        GameObject obstacle = Instantiate(obstaclePrefabs[Random.Range(0, obstaclePrefabs.Count)], transform);
        float range = _bounds.size.x / 2f;
        obstacle.transform.localPosition = new Vector3(0f, 0f, Random.Range(-range, range));
    }
}
