// This crash manager initiates a crash based on 2 values:
// 1: Whether the hit object was low or high
// 2: The current speed
// Both the low and high objects have 3 separate types of crashes, increasingly influential based on the current speed
// Low small or high small: The player just stops, and has to slowly move away from the object
// Low medium: The player falls over the object, the goat stays behind. 2 second penalty
// Low hard: The player somersaults over the object, we see stars above his head. 5 second penalty
// High medium: The player launches back a bit, but stays on the goat. 2 second penalty
// High hard: The player launches back, we see stars above his head. 5 second penalty

using System.Collections.Generic;
using System.Linq;
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
    [SerializeField] private Anim smallForwardAnimation;
    [Header("LowMedium")]
    [SerializeField] private Anim lowMediumForwardAnimation;
    [Header("LowHigh")]
    [SerializeField] private Anim lowHighForwardAnimation;

    private List<Anim> _activeAnims = new List<Anim>();
    
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
        if (_activeAnims.Count > 0)
        {
            Vector3 pos = Vector3.zero;
            Vector3 rot = Vector3.zero;
            foreach (Anim a in _activeAnims.ToList())
            {
                if (!a.playing)
                {
                    _activeAnims.Remove(a);
                    break;
                }
                pos += new Vector3(a.pt[0], a.pt[1], a.pt[2]);
                rot +=  new Vector3(a.rt[0], a.rt[1], a.rt[2]);
            }

            if (_activeAnims.Count <= 0) return;
            transform.localPosition = pos;
            transform.localEulerAngles = rot;
            return;
        }

        // Reset
        transform.localPosition = Vector3.Lerp(transform.localPosition, Vector3.zero, Time.deltaTime * recoverSmoothness);
        transform.localEulerAngles = Vector3.Slerp(transform.localEulerAngles, Vector3.zero, Time.deltaTime * recoverSmoothness);
    }
    
    public void StartAnimation(Anim a)
    {
        _activeAnims.Add(a);
        StartCoroutine(a.Animate());
        player.SetDisableTime(a.time);
    }

    private void Small()
    {
        StartAnimation(smallForwardAnimation);
        print("Small");
    }

    private void LowMedium()
    {
        StartAnimation(lowMediumForwardAnimation);
        print("Low Medium");
    }

    private void LowHard()
    {
        StartAnimation(lowHighForwardAnimation);
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
