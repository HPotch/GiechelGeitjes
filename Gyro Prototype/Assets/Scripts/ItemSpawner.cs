using UnityEngine;

using System.Collections.Generic;

public class ItemSpawner : MonoBehaviour
{
    [SerializeField] private Vector2 spawnDistance = new Vector2(0.3f, 4f);
    [SerializeField] private List<GameObject> itemPrefabs = new List<GameObject>();
    
    private float distance = 0f;
    
    private void Update()
    {
        distance += GameManager.Instance.moveAmount * Time.deltaTime;
        Debug.Log(distance);
    }
}
