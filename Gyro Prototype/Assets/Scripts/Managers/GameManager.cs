using System;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

	[Header("Players")]
	[SerializeField] private Transform player1;
	[SerializeField] private Transform player2;

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

    public Transform Leader() 
    {
		// Find the player thats in front
		Transform leader = player1.position.z >= player2.position.z
			? player1
			: player2;

        return leader;
	}

    private void Update()
    {
		if (player1 == null || player2 == null) return;
		moveAmount -= moveDecay * Time.deltaTime;
        moveAmount = Mathf.Max(moveAmount, 0f);
    }
}
