using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class TargetSpawner : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Vector2 spawnTime = new Vector2(2f, 5f);
    [SerializeField] private int maxTargets = 4;
    [Header("References")]
    [SerializeField] private GameObject targetPrefab;
    [SerializeField] private Transform targetParent;
    [SerializeField] private Transform min;
    [SerializeField] private Transform max;

    private bool _spawning = false;

    private void Update()
    {
        if (_spawning || targetParent.childCount >= maxTargets) return;
        _spawning = true;
        StartCoroutine(spawnRoutine());
    }

    private IEnumerator spawnRoutine()
    {
        float timer = Random.Range(spawnTime.x, spawnTime.y);

        while (timer > 0f)
        {
            timer -= Time.deltaTime;
            yield return null;
        }
        
        _spawning = false;
        SpawnTarget();
    }

    private void SpawnTarget()
    {
        GameObject target = Instantiate(targetPrefab, targetParent);
        target.transform.position = new Vector2(Random.Range(min.position.x, max.position.x), Random.Range(min.position.y, max.position.y));
    }
}
