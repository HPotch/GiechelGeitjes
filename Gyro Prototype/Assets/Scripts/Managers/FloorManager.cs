
using System.Collections.Generic;
using UnityEngine;

public class FloorManager : MonoBehaviour
{
    [Header("Floor Settings")]
    [SerializeField] private GameObject floorPrefab;
    [SerializeField] private int trackWidth = 10;
    [SerializeField] private float tileSize = 1f;
    [SerializeField] private int distanceAhead = 20;
    [SerializeField] private float distanceBehind = 0f;

    private int lastLeaderMeter = int.MinValue;

    // Each entry represents one strip of tiles.
    private readonly Dictionary<int, List<GameObject>> strips =
        new Dictionary<int, List<GameObject>>();


    private void Update()
    {
        if (floorPrefab == null || GameManager.Instance == null)
            return;

        Transform leader = GameManager.Instance.Leader();
        Transform loser = GameManager.Instance.Loser();

        if (leader == null || loser == null)
            return;

        Debug.Log(leader);
        Debug.Log(loser);

        // Only changes when the leader crosses a whole metre.
        int leaderMeter = Mathf.FloorToInt(leader.position.z);

        if (leaderMeter != lastLeaderMeter)
        {
            lastLeaderMeter = leaderMeter;

            // Keep 20 metres of track ahead of the leader.
            int lastZ = leaderMeter + distanceAhead;

            for (int z = leaderMeter; z <= lastZ; z++)
            {
                if (!strips.ContainsKey(z))
                    CreateStrip(z);
            }
        }


        // Remove strips completely behind the last player.
        int loserMeter = Mathf.FloorToInt(loser.position.z);

        List<int> remove = new List<int>();

        foreach (var strip in strips)
        {
            // A strip at z = 10 occupies Z from 10 to 11.
            // Remove it only when its end is behind the loser.
            int stripEndZ = strip.Key + 1;

            if (stripEndZ <= loserMeter)
            {
                remove.Add(strip.Key);
            }
        }

        foreach (int z in remove)
        {
            foreach (GameObject tile in strips[z])
            {
                if (tile != null)
                    Destroy(tile);
            }

            strips.Remove(z);
        }

    }


    private void CreateStrip(int stripZ)
    {
        List<GameObject> tiles = new List<GameObject>();

        // Centre the track around X = 0.
        float startX = -(trackWidth * tileSize) / 2f + tileSize / 2f;

        float centerZ = stripZ * tileSize + tileSize / 2f;

        for (int x = 0; x < trackWidth; x++)
        {
            float centerX = startX + x * tileSize;

            GameObject tile = Instantiate(floorPrefab, new Vector3(centerX, 0f, centerZ), Quaternion.identity, transform);

            tiles.Add(tile);
        }

        strips.Add(stripZ, tiles);
    }
}
