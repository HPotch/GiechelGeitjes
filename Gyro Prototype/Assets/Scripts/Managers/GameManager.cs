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

    public GameObject firstPlayer, secondPlayer;
    
    [Header("Settings")]
    public float maxSpeed = 10f;
    public float bounds = 5f;
    public float sprintMultiplier = 1f;
    public enum MotionSickMode { none, aLittle, aLot }
    [Tooltip("'None' might introduce problems for people who don't regularly experience motion sickness too!")]
    public MotionSickMode motionSickMode = MotionSickMode.none;

    // Public variables
    [NonSerialized] public float moveAmount = 0f;

    private void Start()
    {
        firstPlayer = GameObject.FindGameObjectWithTag("Player");
        secondPlayer = GameObject.FindGameObjectWithTag("Player");
    }
    
    public Transform Leader() 
    {
        // Find the player that is in front
        Transform leader = firstPlayer.transform.position.z >= secondPlayer.transform.position.z
            ? firstPlayer.transform
            : secondPlayer.transform;

        return leader;
    }
}
