// SETTINGS CAN BE FOUND IN GAMEMANAGER

// Deps:
// - Scene should contain a UDPReceiver
// - Scene should contain a GameManager

// This script:
// - Grabs UDP data from the UDPReceiver
// - Interprets movement based on those values
//      - Phone 1 rotates to steer
//      - Phone 2 flicks to move forward

// Public functions:
// - SetDisableTime - Disables the movement for the set amount of time
// - Sprint - Applies speed boost to the player
// - Superjump - Uses animationCurves to animate the player jumping big time

// Public variables
// - moveAmount - The amount the player is currently moving forward in units/second
// - invincible - Bool that can be set and read to make the player invincible (isn't used in this script, though!)
// - breaking - Bool that is set to whether the player is breaking or not

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    /// Public variables ///
    public float moveAmount; // Units/second
    public bool invincible = false;
    public bool breaking = false;
    
    /// Serialized variables ///
    [Header("References")]
    [SerializeField] private Transform jumpTransform;
    public CrashSender crashSender;

    [Header("Settings")]
    [SerializeField] [Range(0, 1)] private int player = 1; // 0 = left; 1 = right

    /// Events ///
    public UnityEvent afterMove; // Called after moving is done

    /// Private variables ///
    private float _tiltVelocity; // Current steering, smoothed based on GameManager.tiltAcceleration
    private float _disableTime; // The current time left until enabled. _disableTime < 0f means movement is enabled
    private float _forwardTimeOut; // Forward motion must use some timeout to work around multiple triggers from one flick
    private float _breakTime = 0f;
    private GameManager _gm;
    
    // Sprint
    private float _sprintMultiplier = 1f; // The current speed multiplier, used by sprint
    private Cam _cam;
    
    
    private void Awake()
    {
        _cam = crashSender.GetComponentInChildren<Cam>();
    }

    private void Start()
    {
        _gm = GameManager.Instance;
    }
    
    
    public void SetDisableTime(float time)
    {
        _disableTime = time;
    }

    /// <summary>
    /// Instantiates a sprint
    /// </summary>
    /// <param name="time"> Sprint time </param>
    /// <param name="smoothing"> Sprint increase / decrease per second, on the start and end of sprint</param>
    /// <param name="multiplier"> Speed multiplier during peak </param>
    /// <param name="fovChange"> How much the camera fov changes</param>
    public void Sprint(float time, float smoothing, float multiplier, float fovChange)
    {
        StartCoroutine(SprintRoutine(time, smoothing, multiplier, fovChange));
    }
    private IEnumerator SprintRoutine(float time, float smoothing, float multiplier, float fovChange)
    {
        if (_sprintMultiplier > 1f) yield break; // Don't execute if already running
        float timer = 0f;
        _sprintMultiplier = 1f;

        // Speed up until time is over
        while (timer < time)
        {
            timer += Time.deltaTime;
            _sprintMultiplier += smoothing * Time.deltaTime;
            _sprintMultiplier = Mathf.Min(_sprintMultiplier, multiplier);
            _cam.additionalFov = Mathf.Lerp(_cam.additionalFov, fovChange, Time.deltaTime * 5f);
            yield return null;
        }

        // Slow down until sprint = 1f
        while (_sprintMultiplier > 1f)
        {
            _sprintMultiplier -= smoothing * Time.deltaTime;
            _cam.additionalFov = Mathf.Lerp(_cam.additionalFov, 0f , Time.deltaTime * 5f); 
            yield return null;
        }
        
        // Exit cleanly
        _sprintMultiplier = 1f;
        _cam.additionalFov = 0f;
    }

    /// <summary>
    /// Instantiates a super jump
    /// </summary>
    /// <param name="time"> Super jump time </param>
    /// <param name="distance"> How far the player travels on the z axis </param>
    /// <param name="height"> Height of the jump </param>
    /// <param name="distanceCurve"> The player will follow this curve on the z axis </param>
    /// <param name="heightCurve"> The player will follow this curve on the y axis </param>
    public void SuperJump(float time, float distance, float height, AnimationCurve distanceCurve, AnimationCurve heightCurve)
    {
        StartCoroutine(SuperJumpRoutine(time, distance, height, distanceCurve, heightCurve));
    }
    private IEnumerator SuperJumpRoutine(float time, float distance, float height, AnimationCurve distanceCurve, AnimationCurve heightCurve)
    {
        float timer = 0f;
        Vector3 start = transform.position;
        float prevZPos = 0f;
        _disableTime = time; // Disable while jumping
        invincible = true; // Cannot get hit during jump

        // Animation
        while (timer < time)
        {
            timer += Time.deltaTime;
            
            float td = distanceCurve.Evaluate(timer / time);
            float th = heightCurve.Evaluate(timer / time);

            prevZPos = transform.position.z;
            transform.position = new Vector3(
                start.x,
                start.y + th * height,
                start.z + td * distance);
            
            afterMove.Invoke();
            yield return null;
        }
        
        moveAmount = (transform.position.z - prevZPos) / Time.deltaTime; // Set the right velocity
        _disableTime = 0f;
        transform.position = new Vector3(start.x, start.y, transform.position.z); // Keep z, the player has traveled, dude

        yield return new WaitForSeconds(1f); // 1 second extra invincibility for when you land right before an obstacle
        invincible = false; // Hit that viking! Hit! Him!
    }
    
    
    private void Update()
    {
        _disableTime -= Time.deltaTime;

        if (_disableTime <= 0f)
        {
            // While wait for other player to reach start, break, else move
            if (!GameManager.Instance.started && transform.position.z > GameManager.Instance.startPosition.position.z)
                SpeedDecay(true); 
            else
                Move();
        }
        else moveAmount = 0f; // Remove all built up velocity
    }

    private void Move()
    {
        // Get values
        UDPReceiver udpr = UDPReceiver.Instance;
        List<float> upd1Values = udpr.func1Values[player].values;
        List<float> upd2Values = udpr.func2Values[player].values;
        
        Steer(GetSteeringInput(upd1Values[0])); // Steer
        SpeedDecay(GetBreakInput(upd2Values[1])); // Speed decay / break
        MoveForward(GetMoveInput(upd2Values[1])); // Forward motion
        
        afterMove.Invoke();
    }

    private float GetSteeringInput(float udpValue)
    {
        // UDP input
        float value = udpValue * _gm.tiltSensitivity;
        
        // Keyboard input
        if (player == 0)
        {
            if (Keyboard.current.leftArrowKey.isPressed) value = -_gm.maxTilt;
            if (Keyboard.current.rightArrowKey.isPressed) value = _gm.maxTilt;
        }
        else
        {
            if (Keyboard.current.aKey.isPressed) value = -_gm.maxTilt;
            if (Keyboard.current.dKey.isPressed) value = _gm.maxTilt;
        }
        
        // Clamp to max tilt
        return Mathf.Clamp(value , -_gm.maxTilt, _gm.maxTilt);
    }

    private void Steer(float dir)
    {
        // Push away from bounds
        float distToLeftBound = GameManager.Instance.bounds - transform.position.x * -1f;
        dir += Mathf.Clamp01(_gm.pushBackDistance - distToLeftBound) * _gm.maxTilt;
        float distToRightBound = GameManager.Instance.bounds - transform.position.x;
        dir -= Mathf.Clamp01(_gm.pushBackDistance - distToRightBound) * _gm.maxTilt;

        // Smoothly interpret movement
        _tiltVelocity = Mathf.Lerp(_tiltVelocity, dir, Time.deltaTime * _gm.tiltAcceleration);
        
        // Apply
        transform.position += new Vector3(_tiltVelocity * Time.deltaTime, 0f, 0f);
    }

    private bool GetMoveInput(float udpValue)
    {
        // UDP input
        bool value = udpValue > _gm.forwardMotionRequired;
        
        // Keyboard input
        if (player == 0)
            value = Keyboard.current.enterKey.wasPressedThisFrame || Mouse.current.rightButton.wasPressedThisFrame || value;
        else
            value = Keyboard.current.wKey.wasPressedThisFrame || Mouse.current.leftButton.wasPressedThisFrame || value;
        
        // Disable if timeout
        _forwardTimeOut -= Time.deltaTime;
        if (_forwardTimeOut > 0f) value = false;
        if (value) _forwardTimeOut = _gm.forwardTimeOut;
        
        return value;
    }
    
    private void MoveForward(bool move)
    {
        // Return if colliding with an object
        if (Physics.Raycast(
            transform.position,
            transform.TransformDirection(Vector3.forward),
            0.1f,
            _gm.layerMask)
            ) return;
        
        // Add speed
        if (move)
        {
            moveAmount += _gm.speedScale;
            moveAmount = Mathf.Min(moveAmount, GameManager.Instance.maxSpeed);
        }
        
        // Apply forward motion
        transform.position += transform.forward * (moveAmount * Time.deltaTime * _sprintMultiplier);
    }

    private bool GetBreakInput(float udpValue)
    {
        // UDP input
        bool value = udpValue < _gm.breakRotationRequired;
        
        // Keyboard input
        if (player == 0)
            value = Keyboard.current.rightShiftKey.isPressed || value;
        else
            value = Keyboard.current.leftShiftKey.isPressed || value;

        if (!value) _breakTime = 0f;
        else _breakTime += Time.deltaTime;
        value = _breakTime > _gm.breakTime;
            
        breaking = value;
        return value;
    }

    private void SpeedDecay(bool useBreak = false)
    {
        if (useBreak)
            moveAmount -= _gm.moveDecay * _gm.breakAmount * Time.deltaTime;
        else
            moveAmount -= _gm.moveDecay * Time.deltaTime;
        
        moveAmount = Mathf.Max(moveAmount, 0f);
    }
}
