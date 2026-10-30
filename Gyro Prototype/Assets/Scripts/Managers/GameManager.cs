using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    [Header("References")]
    [SerializeField] private GameObject firstPlayer, secondPlayer;

    [Header("Bounds")]
    public float bounds = 5f;
    public float pushBackDistance = 1f;

    [Header("Forward")]
    public float forwardMotionRequired = 10f;
    public float maxSpeed = 10f;
    public float speedScale = 1f;
    public float moveDecay = 0.1f; // In degrees
    public float forwardTimeOut = 0.3f; // In seconds
    public LayerMask layerMask;

    [Header("Tilt")]
    public float tiltSensitivity = 3f;
    public float maxTilt = 0.2f;
    public float tiltAcceleration = 10f;
    
    [Header("Break")]
    public float breakRotationRequired = -0.8f;
    [Tooltip("Break amount is compared to regular speed decay")]
    public float breakAmount = 3f;
    [Tooltip("The time the player should break before actually executing")] // Helps prevent accidental breaks or inaccuracies
    public float breakTime = 0.35f; // In seconds
    
    public enum MotionSickMode { none, aLittle, aLot }
    [Header("MotionSick")]
    [Tooltip("'None' might introduce problems for people who don't regularly experience motion sickness too!")]
    public MotionSickMode motionSickMode = MotionSickMode.none;

    [Header("Start")]
    public bool started = false;
    public Transform startPosition;

    private void Start()
    {
        if (Display.displays.Length > 1)
            Display.displays[1].Activate();
        if (Display.displays.Length > 2)
            Display.displays[2].Activate();
    }
    
    private void Update()
    {
        /*Debug.Log(
            $"First Player: {(firstPlayer ? firstPlayer.name : "NULL")} " +
            $"Z: {(firstPlayer ? firstPlayer.transform.position.z : 0f)}, " +
            $"Second Player: {(secondPlayer ? secondPlayer.name : "NULL")} " +
            $"Z: {(secondPlayer ? secondPlayer.transform.position.z : 0f)}"
        );*/

        if (firstPlayer.transform.position.z >= startPosition.position.z &&
            secondPlayer.transform.position.z >= startPosition.position.z) StartManager.Instance.StartGame();
    }

    public Transform Leader()
    {
        if (!firstPlayer || !secondPlayer)
            return null;

        return firstPlayer.transform.position.z >= secondPlayer.transform.position.z
            ? firstPlayer.transform
            : secondPlayer.transform;
    }

    public Transform Loser()
    {
        if (!firstPlayer || !secondPlayer)
            return null;

        return firstPlayer.transform.position.z < secondPlayer.transform.position.z
            ? firstPlayer.transform
            : secondPlayer.transform;
    }
}
