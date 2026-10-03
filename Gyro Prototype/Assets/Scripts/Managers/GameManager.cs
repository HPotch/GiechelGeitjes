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

    [Header("Settings")]
    [SerializeField] private float speedScale = 1f;
    [SerializeField] private float moveDecay = 0.1f;
    public float maxSpeed = 10f;
    public float bounds = 5f;
    public enum MotionSickMode { none, aLittle, aLot }
    [Tooltip("'None' might introduce problems for people who don't regularly experience motion sickness too!")]
    public MotionSickMode motionSickMode = MotionSickMode.none;

    // Public variables
    [NonSerialized] public float moveAmount = 0f;

    public void Move(float amount)
    {
        moveAmount += amount * speedScale;
        moveAmount = Mathf.Min(moveAmount, maxSpeed);
    }

    private void Update()
    {
        moveAmount -= moveDecay * Time.deltaTime;
        moveAmount = Mathf.Max(moveAmount, 0f);
    }
}
