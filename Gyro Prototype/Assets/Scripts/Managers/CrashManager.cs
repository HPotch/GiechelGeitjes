// This crash manager initiates a crash based on 2 values:
// 1: Whether the hit object was low or high
// 2: The current speed
// Both the low and high objects have 3 separate types of crashes, increasingly influential based on the current speed
// Low small or high small: The player just stops, and has to slowly move away from the object
// Low medium: The player falls over the object, the goat stays behind. 2 second penalty
// Low hard: The player somersaults over the object, we see stars above his head. 5 second penalty
// High medium: The player launches back a bit, but stays on the goat. 2 second penalty
// High hard: The player launches back, we see stars above his head. 5 second penalty

using UnityEngine;

public class CrashManager : MonoBehaviour
{
    public static CrashManager Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else  Destroy(gameObject);
    }

    public enum CrashType
    {
        Low, High
    }
    
    [Header("References")]
    [SerializeField] private PlayerMovement player;
    [Header("Settings")]
    [SerializeField] private float noCrashUntil = 0.1f;
    [SerializeField] private float smallCrashUntil = 0.4f;
    [SerializeField] private float mediumCrashUntil = 0.85f;
    [SerializeField] private float recoverSmoothness = 10f;
    [Header("Small")]
    [SerializeField] private float smallDisableTime = 0.3f;
    [SerializeField] private Animation smallForwardAnimation;
    [Header("LowMedium")]
    [SerializeField] private float lowMediumDisableTime = 1f;
    [SerializeField] private Animation lowMediumForwardAnimation;
    [Header("LowHigh")]
    [SerializeField] private float lowHighDisableTime = 2f;
    [SerializeField] private Animation lowHighForwardAnimation;

    private float _forwardForce = 0f;
    
    // Additional speed is used for crashes with incoming traffic, for example. The speed of the player and of the other moving object should be added together.
    public void Crash(CrashType type, float additionalSpeed = 0f)
    {
        float speedFraction = (GameManager.Instance.moveAmount + additionalSpeed) / GameManager.Instance.maxSpeed;
        
        GameManager.Instance.moveAmount = 0f;

        if (speedFraction <= noCrashUntil) return;
        
        if (type == CrashType.Low)
        {
            if (speedFraction <= smallCrashUntil)
                Small();
            else if (speedFraction <= mediumCrashUntil)
                LowMedium();
            else
                LowHard();
        }
        else if (type == CrashType.High)
        {
            if (speedFraction <= smallCrashUntil)
                Small();
            else if (speedFraction <= mediumCrashUntil)
                HighMedium();
            else
                HighHard();
        }
    }

    private void Update()
    {
        if (_forwardForce > 0f)
            transform.localPosition += Vector3.forward * (_forwardForce * Time.deltaTime);
        else
            transform.localPosition = Vector3.Lerp(transform.localPosition, Vector3.zero, recoverSmoothness * Time.deltaTime);
    }

    private void Small()
    {
        player.SetDisableTime(smallDisableTime);
        print("Small");
    }

    private void LowMedium()
    {
        player.SetDisableTime(lowMediumDisableTime);
        print("Low Medium");
    }

    private void LowHard()
    {
        player.SetDisableTime(lowHighDisableTime);
        print("Low Hard");
    }

    private void HighMedium()
    {
        print("High Medium");
    }

    private void HighHard()
    {
        print("High Hard");
    }
}
