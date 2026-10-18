using UnityEngine;

[ExecuteAlways]
public class GroundSpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject stripPrefab;
    
    [Header("Settings")]
    [SerializeField] private float lookAheadDistance = 20f;
    [SerializeField] private float behindDistance = 2f;
    [SerializeField] private float tileSize = 1f;

    private float _furthestDistance;
    private Transform _firstPlayer, _secondPlayer;

    private void Start()
    {
        _firstPlayer = GameManager.Instance.firstPlayer.transform;
        _secondPlayer = GameManager.Instance.secondPlayer.transform;
        
    }

    private void Update()
    {
        // If firstplayer.z > _furthestDistance + tileSize spawn a new strip and round it to the tileSize
        // Always round furthestdistance to tileSize
        // like this:
        // furthestdistance = Mathf.Round(_furthestDistance * tilesize) / tileSize
        // 
        // Then we need a new script on the script checking if it is further then behindDistance behind the secondPlayer
    }
}
