using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    [SerializeField] private GameObject firstPlayer, secondPlayer;
    
    [Header("Settings")]
    public float maxSpeed = 10f;
    public float bounds = 5f;
    public float sprintMultiplier = 1f;


    public enum MotionSickMode { none, aLittle, aLot }
    [Tooltip("'None' might introduce problems for people who don't regularly experience motion sickness too!")]
    public MotionSickMode motionSickMode = MotionSickMode.none;

    private void Update()
    {
        Debug.Log(
            $"First Player: {(firstPlayer != null ? firstPlayer.name : "NULL")} " +
            $"Z: {(firstPlayer != null ? firstPlayer.transform.position.z : 0f)}, " +
            $"Second Player: {(secondPlayer != null ? secondPlayer.name : "NULL")} " +
            $"Z: {(secondPlayer != null ? secondPlayer.transform.position.z : 0f)}"
        );
    }

    public Transform Leader()
    {
        if (firstPlayer == null || secondPlayer == null)
            return null;

        return firstPlayer.transform.position.z >= secondPlayer.transform.position.z
            ? firstPlayer.transform
            : secondPlayer.transform;
    }

    public Transform Loser()
    {
        if (firstPlayer == null || secondPlayer == null)
            return null;

        return firstPlayer.transform.position.z < secondPlayer.transform.position.z
            ? firstPlayer.transform
            : secondPlayer.transform;
    }
}
