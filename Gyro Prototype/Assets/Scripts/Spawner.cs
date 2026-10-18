using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Spawner : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Vector2 spawnDistance = new Vector2(0.3f, 4f);
    [Header("References")]
    [SerializeField] private List<GameObject> prefabs = new List<GameObject>();

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
            // distance -= GameManager.Instance.moveAmount * Time.deltaTime; < Used to be this
            distance -= 0;
            yield return null;
        }
        
        _spawning = false;
        SpawnObstacle();
    }

    private void SpawnObstacle()
    {
        GameObject obstacle = Instantiate(prefabs[Random.Range(0, prefabs.Count)], transform);
        float range = GameManager.Instance.bounds;
        obstacle.transform.localPosition = new Vector3(0f, 0f, Random.Range(-range, range));
    }
}
