// Destroyer goats are spawned whenever a Player picks up a horn
// This feature introduces rubber banding baked into the mechanics:
// 1) Whenever the first player spawns destroyer goats, everything from the last player to somewhere in front of the first player gets destroyed,
// meaning that the last player actually profits more from it
// 2) Whenever the last player spawns destroyer goats, the first player's gotta be really close to the last player to profit from the spawns

using System;
using System.Collections;
using UnityEngine;

public class DestroyerGoat : MonoBehaviour
{
    [Header("References")]
    [NonSerialized] public Transform player;

    [Header("Settings")]
    [SerializeField] private float clippingBounds = 20f; // The distance behind the horizon
    [SerializeField] private float runToPlayerTime = 2f;
    [SerializeField] private AnimationCurve runToPlayerCurve;
    [SerializeField] private float matchPlayerTime = 1f;
    [SerializeField] private float runFromPlayerTime = 2f;
    [SerializeField] private AnimationCurve runFromPlayerCurve;
    [SerializeField] private float runToSidesTime = 2f;
    [SerializeField] private AnimationCurve runToSidesCurve;
    [SerializeField] private int maxHitPoints = 3; // The max hits the goat can take AFTER HAVING ARRIVED AT THE TARGET PLAYER

    private float _startX = 0f;
    private int _hitPoints;
    private bool _useHitPoints = false;

    private void Awake()
    {
        _hitPoints = maxHitPoints;
    }

    public void Hit()
    {
        if (!_useHitPoints) return;
        _hitPoints--;
        if (_hitPoints <= 0)
            Destroy(gameObject);
    }
    
    public void StartRun()
    {
        StartCoroutine(Run());
    }
    
    private IEnumerator Run()
    {
        _startX = transform.position.x - player.transform.position.x; // Set start X to be relative to the player
        // Spawn 20 units behind the second player
        float startPos = (GameManager.Instance.Loser().position.z - clippingBounds) - player.transform.position.z;
        float distToPlayer = startPos;

        // Step 1: move towards the target player
        float timer = 0f;

        while (timer < runToPlayerTime)
        {
            timer += Time.deltaTime;
            float s = runToPlayerCurve.Evaluate(timer / runToPlayerTime);
            distToPlayer = Mathf.Lerp(startPos, 0f, s);
            SetPos(distToPlayer);
            yield return null;
        }
        distToPlayer = 0f; // Exit cleanly
        _useHitPoints = true;
        
        // Step 2: Wait there
        timer = 0f;
        while (timer < matchPlayerTime)
        {
            timer += Time.deltaTime;
            SetPos(distToPlayer);
            yield return null;
        }
        
        // Step 3: Run away from the target player
        timer = 0f;
        while (timer < runFromPlayerTime)
        {
            timer += Time.deltaTime;
            float s = runFromPlayerCurve.Evaluate(timer / runFromPlayerTime);
            distToPlayer = Mathf.Lerp(0f, clippingBounds, s);
            SetPos(distToPlayer);
            yield return null;
        }
        
        // Step 4: Run to the sides, so a destroyer goat doesn't just despawn were a player to see them at the end of their run
        timer = 0f;
        while (timer < runToSidesTime)
        {
            timer += Time.deltaTime;
            float s = runToSidesCurve.Evaluate(timer / runToSidesTime);
            SetPos(distToPlayer, s * clippingBounds);
            yield return null;
        }
        
        // End
        Destroy(gameObject);
    }

    private void SetPos(float p, float side = 0f)
    {
        // Both x and z move relative to the player
        float x = _startX;
        if (side != 0f)
            x = transform.position.x > 0f ? x + side : x - side; // Used transform.position.x instead of just x so that the destroyer goat always cuts out at the horizon.
        
        // Set the position relative to the target Player
        transform.position = new Vector3(player.transform.position.x + x, transform.position.y, player.transform.position.z + p);
        
    }
}
