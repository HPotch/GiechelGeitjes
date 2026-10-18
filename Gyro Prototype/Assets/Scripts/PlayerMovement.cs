using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("References")] 
    [SerializeField] private Transform jumpTransform;
    
    [Header("Settings")]
    [SerializeField] [Tooltip("This curve decides tilt speed, based on the GameManagers max speed")] private AnimationCurve tiltCurve;
    [SerializeField] [Tooltip("The player gets pushed back if they are this close to the bounds")] private float pushBackDistance = 1f;
    [SerializeField] [Tooltip("The layer mask used for detecting if the player can move")] private LayerMask layerMask;
    
    private enum MoveMode { viking, goat }
    
    [Header("Forward")]
    [SerializeField] private float forwardMotionRequired = 10f; // In degrees
    [SerializeField] [Tooltip("The viking's gyro must go from top to bottom in this time for a move to occur")] private float forwardSpeedRequired = 0.7f; // In seconds
    [SerializeField] [Tooltip("Time == vikingSpeedRequired means a multiplier of 1, Time == 0f means a multiplier of this value")] private float forwardSpeedMultiplier = 2f;
    
    [Header("Jump")]
    [SerializeField] private float jumpMotionRequired = 1f;
    [SerializeField] private float jumpTimeOut = 1f;
    [SerializeField] private Anim jumpAnimation;
    
    [Header("Tilt")]
    [SerializeField] private float tiltSensitivity = 3f;
    
    [Header("Phone Functions")]
    [SerializeField] private MoveMode phone1Mode;
    [SerializeField] private MoveMode phone2Mode;
    [SerializeField] private int group = 1;
    
    public UnityEvent afterMove;

    private float _disableTime, _jumpDisableTime;

    public void SetDisableTime(float time)
    {
        _disableTime = time;
    }

    private void Update()
    { 
        _disableTime -= Time.deltaTime;
        _jumpDisableTime -= Time.deltaTime;
        
        if (_disableTime > 0f) return;
        UDPReceiver udpr = UDPReceiver.Instance;
        List<float> a1v = udpr.func1Values[group].values;
        List<float> a2v = udpr.func2Values[group].values;
        
        // Phone controls
        float goatDirection = phone1Mode == MoveMode.goat ? a1v[0] : a2v[0];
        float forwardDirection = phone1Mode == MoveMode.viking ? a1v[2] : a2v[2];
        float jumpDirection = phone1Mode == MoveMode.viking ? a1v[0] : a2v[0];
        bool forward = GetVikingSpeed(forwardDirection);
        
        // Keyboard controls
        if (Keyboard.current.leftArrowKey.isPressed || Keyboard.current.aKey.isPressed)
            goatDirection = -0.2f;
        if (Keyboard.current.rightArrowKey.isPressed || Keyboard.current.dKey.isPressed)
            goatDirection = 0.2f;
        if (Keyboard.current.spaceKey.wasPressedThisFrame || Mouse.current.leftButton.wasPressedThisFrame)
            forward = true;

        bool jump = Keyboard.current.enterKey.wasPressedThisFrame || jumpDirection > 0.3f;
        
        LeftRight(goatDirection);
        if (forward) Forward();
        Jump(jump);
    }
    
    private bool GetVikingSpeed(float dir)
    {
        return dir > forwardMotionRequired;
    }
    
    private void LeftRight(float dir)
    {
        float direction = Mathf.Clamp(dir, -1f, 1f);

        float distToLeftBound = GameManager.Instance.bounds - transform.position.x * -1f;
        direction += Mathf.Clamp01(pushBackDistance - distToLeftBound);
        float distToRightBound = GameManager.Instance.bounds - transform.position.x;
        direction -= Mathf.Clamp01(pushBackDistance - distToRightBound);

        float tiltSpeed = tiltCurve.Evaluate(Mathf.Clamp01(GameManager.Instance.moveAmount / GameManager.Instance.maxSpeed));
        
        float velocity = direction * tiltSpeed * tiltSensitivity * Time.deltaTime;

        transform.position += new Vector3(velocity, 0f, 0f);
        afterMove.Invoke();
    }
    
    private void Forward(float sp = 1f)
    {
        if (CastRay()) return;
        GameManager.Instance.Move(sp);
    }

    private void Jump(bool initiateJump)
    {
        // Perform jump
        if (jumpAnimation.playing)
        {
            jumpTransform.localPosition = jumpAnimation.pt;
            jumpTransform.localEulerAngles =  jumpAnimation.rt;
        }
        else
        {
            jumpTransform.localPosition = Vector3.Lerp(jumpTransform.localPosition, Vector3.zero, 20f * Time.deltaTime);
            jumpTransform.localEulerAngles = Vector3.Lerp(jumpTransform.eulerAngles, Vector3.zero, 20f * Time.deltaTime);
        }
        
        // Init jump
        if (!initiateJump || _jumpDisableTime > 0f) return;
        _jumpDisableTime = jumpTimeOut;
        StartCoroutine(jumpAnimation.Animate());
    }

    private bool CastRay()
    {
        return Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), 1f, layerMask);
    }

    public IEnumerator Sprint(float time, float smoothing, float multiplier)
    {
        if (GameManager.Instance.sprintMultiplier > 1f) yield break; // Don't execute if already running
        float timer = 0f;
        GameManager.Instance.sprintMultiplier = 1f;

        // Speed up until time is over
        while (timer < time)
        {
            timer += Time.deltaTime;
            GameManager.Instance.sprintMultiplier += smoothing * Time.deltaTime;
            GameManager.Instance.sprintMultiplier = Mathf.Min(GameManager.Instance.sprintMultiplier, multiplier);
            yield return null;
        }

        // Slow down until sprint = 1f
        while (GameManager.Instance.sprintMultiplier > 1f)
        {
            GameManager.Instance.sprintMultiplier -= smoothing * Time.deltaTime;
            yield return null;
        }

        // Exit cleanly
        GameManager.Instance.sprintMultiplier = 1f;
    }
}
