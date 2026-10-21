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

public class CrashSender : MonoBehaviour
{
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
    [SerializeField] private Anim smallAnimation;
    [Header("LowMedium")]
    [SerializeField] private Anim lowMediumAnimation;
    [Header("LowHard")]
    [SerializeField] private Anim lowHardAnimation;
    [Header("HighMedium")]
    [SerializeField] private Anim highMediumAnimation;
    [Header("HighHard")]
    [SerializeField] private Anim highHardAnimation;

    private List<Anim> _activeAnims = new List<Anim>();
    
    // Additional speed is used for crashes with incoming traffic, for example. The speed of the player and of the other moving object should be added together.
    public void Crash(CrashType type, float additionalSpeed = 0f)
    {
        float speedFraction = (player.moveAmount + additionalSpeed) / GameManager.Instance.maxSpeed;
        
        player.moveAmount = 0f;

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
                    SetPosition(); // Set the position to the current animation position. This way we can continue riding after falling over something.
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

    private void SetPosition()
    {
        player.transform.position = new Vector3(
            player.transform.position.x,
            player.transform.position.y,
            transform.position.z);

        // Reset z so that it doesn't jitter back
        transform.localPosition = new Vector3(
            transform.localPosition.x,
            transform.localPosition.y,
            0f);
    }
    
    
    public void StartAnimation(Anim a)
    {
        _activeAnims.Add(a);
        StartCoroutine(a.Animate());
        player.SetDisableTime(a.time);
    }

    private void Small()
    {
        StartAnimation(smallAnimation);
        //print("Small");
    }

    private void LowMedium()
    {
        StartAnimation(lowMediumAnimation);
        //print("Low Medium");
    }

    private void LowHard()
    {
        StartAnimation(lowHardAnimation);
        //print("Low Hard");
    }

    private void HighMedium()
    {
        StartAnimation(highMediumAnimation);
        //print("High Medium");
    }

    private void HighHard()
    {
        StartAnimation(highHardAnimation);
        //print("High Hard");
    }
}
