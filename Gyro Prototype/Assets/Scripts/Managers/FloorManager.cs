using System.Collections.Generic;
using UnityEngine;

public class FloorManager : MonoBehaviour
{
    [Header("Floor Prefab")]
    [SerializeField] private GameObject floorPrefab;

    [Header("Track Settings")]
    [SerializeField] private float stripLength = 1f;
    [SerializeField] private float distanceAhead = 20f;
    [SerializeField] private float distanceBehind = 2f;
    // Strip transform settings are now adjustable in the prefab. This way we prevent generating a new mesh each time.

    // Each strip is stored by its integer world-space Z position.
    private readonly Dictionary<int, GameObject> strips =
        new Dictionary<int, GameObject>();

    private void Update()
    {
        if (floorPrefab == null)
        {
            Debug.LogWarning("FloorManager: Floor Prefab is NOT assigned!");
            return;
        }

        if (GameManager.Instance == null)
        {
            Debug.LogWarning("FloorManager: GameManager.Instance is NULL!");
            return;
        }

        Transform leader = GameManager.Instance.Leader();
        Transform loser = GameManager.Instance.Loser();

        if (leader == null || loser == null)
        {
            Debug.LogWarning(
                "FloorManager: Leader or Loser is NULL! Check player assignments."
            );
            return;
        }

        // Find the first strip we need to keep.
        int firstStrip = Mathf.FloorToInt(
            (loser.position.z - distanceBehind) / stripLength
        );

        // Find the end of the range ahead of the leader.
        int endStrip = Mathf.CeilToInt(
            (leader.position.z + distanceAhead) / stripLength
        );

        // Create missing strips.
        for (int z = firstStrip; z < endStrip; z++)
        {
            if (!strips.ContainsKey(z))
                CreateStrip(z);
        }
        
        Debug.Log($"FloorManager: Leader Z={leader.position.z:F2}, " + $"Loser Z={loser.position.z:F2}, " + $"Required strips={firstStrip} to {endStrip - 1}, " + $"Existing strips={strips.Count}");

        // Remove strips outside the required range.
        List<int> remove = new List<int>();

        foreach (KeyValuePair<int, GameObject> strip in strips)
        {
            if (strip.Key < firstStrip || strip.Key >= endStrip)
                remove.Add(strip.Key);
        }

        foreach (int z in remove)
        {
            if (strips[z] != null)
                Destroy(strips[z]);

            strips.Remove(z);
        }
    }

    private void CreateStrip(int z)
    {
        // Each strip occupies [z, z + stripLength] along world Z.
        Vector3 position = new Vector3(
            0f,
            0f,
            z * stripLength + stripLength / 2f
        );

        GameObject strip = Instantiate(
            floorPrefab,
            position,
            Quaternion.identity,
            transform
        );

        strips.Add(z, strip);
    }

    private void OnDestroy()
    {
        foreach (GameObject strip in strips.Values)
        {
            if (strip != null)
                Destroy(strip);
        }

        strips.Clear();
    }
}
