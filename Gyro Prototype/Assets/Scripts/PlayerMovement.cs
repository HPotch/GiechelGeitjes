using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] [Tooltip("Units per second")] private float speed = 2f;
    [SerializeField] [Tooltip("This curve decides tilt speed, based on the GameManagers max speed")] private AnimationCurve tiltCurve;
    [SerializeField] [Tooltip("The player gets pushed back if they are this close to the bounds")] private float pushBackDistance = 1f;
    [SerializeField] [Tooltip("The layermask used for detecting if the player can move")] private LayerMask layerMask;
    
    public UnityEvent afterMove;

    private float _disableTime = 0f;

    public void SetDisableTime(float time)
    {
        _disableTime = time;
    }
    
    private void Update()
    { 
        _disableTime -= Time.deltaTime;
        if (_disableTime > 0f) return;
        LeftRight();
        Forward();
    }

    private void LeftRight()
    {
        float dir = 0f;
        if (Keyboard.current.leftArrowKey.isPressed || Keyboard.current.aKey.isPressed)
            dir = -1;
        if (Keyboard.current.rightArrowKey.isPressed || Keyboard.current.dKey.isPressed)
            dir = 1;

        float distToLeftBound = GameManager.Instance.bounds - transform.position.x * -1f;
        dir += Mathf.Clamp01(pushBackDistance - distToLeftBound);
        float distToRightBound = GameManager.Instance.bounds - transform.position.x;
        dir -= Mathf.Clamp01(pushBackDistance - distToRightBound);

        float tiltSpeed = tiltCurve.Evaluate(Mathf.Clamp01(GameManager.Instance.moveAmount / GameManager.Instance.maxSpeed));
        
        float velocity = dir * Time.deltaTime * speed * tiltSpeed;

        transform.position += new Vector3(velocity, 0f, 0f);
        afterMove.Invoke();
    }

    private void Forward()
    {
        if (!(Keyboard.current.spaceKey.wasPressedThisFrame || Mouse.current.leftButton.wasPressedThisFrame)) return;
        if (CastRay()) return;
        
        GameManager.Instance.Move(1f);
    }

    private bool CastRay()
    {
        return Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), 1f, layerMask);
    }
}
