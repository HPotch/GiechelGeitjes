using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerMovementOld : MonoBehaviour
{
    public float moveAmount;
    public bool invincible = false;
    
	private enum MoveMode { viking, goat }

	private enum GoatPhoneMode { vertical, horizontal }

    /// Serialized variables ///
    [Header("References")]
    [SerializeField] private Transform jumpTransform;
    public CrashSender crashSender;

    [Header("Settings")]
    [SerializeField][Tooltip("This curve decides tilt speed, based on the GameManagers max speed")] private AnimationCurve tiltCurve;
    [SerializeField][Tooltip("The player gets pushed back if they are this close to the bounds")] private float pushBackDistance = 1f;
    [SerializeField][Tooltip("The layer mask used for detecting if the player can move")] private LayerMask layerMask;
    [SerializeField] private int player = 1;

    [Header("Forward")]
    [SerializeField] private float forwardMotionRequired = 10f; // In degrees
    [SerializeField] private float speedScale = 1f;
    [SerializeField] private float moveDecay = 0.1f;
    public float maxSpeed = 10f;
    
    [Header("Tilt")]
    [SerializeField] private float tiltSensitivity = 3f;

    [SerializeField] private float maxTilt = 0.2f;

    [Header("Phone Functions")]
    [SerializeField] private MoveMode phone1Mode;
    [SerializeField] private MoveMode phone2Mode;

    /// Private variables ///
    public UnityEvent afterMove;

    // Hit
    private float _disableTime;
    
    // Sprint
    private float _sprintMultiplier = 1f;
    private Camera _cam;
    private float _startFov;

    private void Awake()
    {
        _cam = crashSender.GetComponentInChildren<Camera>();
        _startFov = _cam.fieldOfView;
    }

    private void Start()
    {
        maxSpeed = GameManager.Instance.maxSpeed;
    }

    public void SetDisableTime(float time)
    {
        _disableTime = time;
    }
    
    private void Update()
    {
        moveAmount -= moveDecay * Time.deltaTime;
        moveAmount = Mathf.Max(moveAmount, 0f);
        
        _disableTime -= Time.deltaTime;

        if (_disableTime > 0f) return;
        UDPReceiver udpr = UDPReceiver.Instance;
        List<float> a1v = udpr.func1Values[player].values; // a1v = short for Attitude 1 Values
        List<float> a2v = udpr.func2Values[player].values; // a2v, well you can figure it out

        // Phone controls
        float goatDirection = phone1Mode == MoveMode.goat ? a1v[0] : a2v[0];
        float forwardDirection = phone1Mode == MoveMode.viking ? a1v[2] : a2v[2];
        bool forward = GetVikingSpeed(forwardDirection);

        // Keyboard controls
        if (player == 0)
        {
            if (Keyboard.current.leftArrowKey.isPressed)
                goatDirection = -1f;
            if (Keyboard.current.rightArrowKey.isPressed)
                goatDirection = 1f;
            if (Keyboard.current.rightShiftKey.wasPressedThisFrame || Mouse.current.rightButton.wasPressedThisFrame)
                forward = true;
        }
        else
        {
            if (Keyboard.current.aKey.isPressed)
                goatDirection = -1f;
            if (Keyboard.current.dKey.isPressed)
                goatDirection = 1f;
            if (Keyboard.current.leftShiftKey.wasPressedThisFrame || Mouse.current.leftButton.wasPressedThisFrame)
                forward = true;
        }

        LeftRight(goatDirection);
        if (forward) Forward();
        MoveForward();

        //Debug.Log($"{name} Z: {transform.position.z}, " + $"Parent Z: {transform.parent.position.z}");
        afterMove.Invoke();
    }

    private bool GetVikingSpeed(float dir)
    {
        return dir > forwardMotionRequired;
    }

    private void LeftRight(float dir)
    {
        float direction = Mathf.Clamp(dir, -maxTilt, maxTilt);

        float distToLeftBound = GameManager.Instance.bounds - transform.position.x * -1f;
        direction += Mathf.Clamp01(pushBackDistance - distToLeftBound);
        float distToRightBound = GameManager.Instance.bounds - transform.position.x;
        direction -= Mathf.Clamp01(pushBackDistance - distToRightBound);

        float tiltSpeed = tiltCurve.Evaluate(Mathf.Clamp01(moveAmount / GameManager.Instance.maxSpeed)); // x:0,y:~0.2 ; x:1,y:1 // kinda like an exp curve

        float velocity = direction * tiltSpeed * tiltSensitivity * Time.deltaTime;

        transform.position += new Vector3(velocity, 0f, 0f);
    }

    private void Forward(float sp = 1f)
    {
        if (CastRay()) return;
        moveAmount += sp * speedScale;
        moveAmount = Mathf.Min(moveAmount, maxSpeed);
    }

    private void MoveForward()
    {
		transform.position += transform.forward * (moveAmount * Time.deltaTime * _sprintMultiplier);
	}

    private bool CastRay()
    {
        return Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), 1f, layerMask);
    }

    public void Sprint(float time, float smoothing, float multiplier, float fovChange)
    {
        StartCoroutine(SprintRoutine(time, smoothing, multiplier, fovChange));
    }
    
    public IEnumerator SprintRoutine(float time, float smoothing, float multiplier, float fovChange)
    {
        if (_sprintMultiplier > 1f) yield break; // Don't execute if already running
        float timer = 0f;
        _sprintMultiplier = 1f;

        // Speed up until time is over
        while (timer < time)
        {
            timer += Time.deltaTime;
            print(timer.ToString() + "   " + time.ToString());
            _sprintMultiplier += smoothing * Time.deltaTime;
            _sprintMultiplier = Mathf.Min(_sprintMultiplier, multiplier);
            _cam.fieldOfView = Mathf.Lerp(_cam.fieldOfView, _startFov + fovChange, Time.deltaTime * 5f);
            yield return null;
        }

        // Slow down until sprint = 1f
        while (_sprintMultiplier > 1f)
        {
            _sprintMultiplier -= smoothing * Time.deltaTime;
            _cam.fieldOfView = Mathf.Lerp(_cam.fieldOfView, _startFov , Time.deltaTime * 5f); 
            yield return null;
        }
        
        // Exit cleanly
        _sprintMultiplier = 1f;
        _cam.fieldOfView = _startFov;
    }

    public void SuperJump(float time, float distance, float height, AnimationCurve distanceCurve, AnimationCurve heightCurve)
    {
        StartCoroutine(SuperJumpRoutine(time, distance, height, distanceCurve, heightCurve));
    }

    private IEnumerator SuperJumpRoutine(float time, float distance, float height, AnimationCurve distanceCurve, AnimationCurve heightCurve)
    {
        float timer = 0f;
        Vector3 start = transform.position;
        _disableTime = time; // Disable while jumping
        invincible = true; // Cannot get hit during jump

        // Animation
        while (timer < time)
        {
            timer += Time.deltaTime;
            
            float td = distanceCurve.Evaluate(timer / time);
            float th = heightCurve.Evaluate(timer / time);
            transform.position = new Vector3(
                start.x,
                start.y + th * height,
                start.z + td * distance);
            
            afterMove.Invoke();
            
            yield return null;
        }
        
        transform.position = new Vector3(start.x, start.y, transform.position.z); // Keep z, the player has traveled, dude

        yield return new WaitForSeconds(1f); // 1 second extra invincibility for when you land right before an obstacle
        invincible = false; // Hit that viking! Hit! Him!
    }
}
