using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Tooltip("How far the player can move")] public float bounds = 5f;
    [SerializeField] [Tooltip("Units per second")] private float speed = 2f;
    [SerializeField] [Tooltip("The player gets pushed back if they are this close to the bounds")] private float pushBackDistance = 1f;

    public UnityEvent afterMove;
    
    private void Update()
    { 
        float dir = 0f;
        if (Keyboard.current.leftArrowKey.isPressed || Keyboard.current.aKey.isPressed)
            dir = -1;
        if (Keyboard.current.rightArrowKey.isPressed || Keyboard.current.dKey.isPressed)
            dir = 1;

        float distToLeftBound = bounds - transform.position.x * -1f;
        dir += Mathf.Clamp01(pushBackDistance - distToLeftBound);
        float distToRightBound = bounds - transform.position.x;
        dir -= Mathf.Clamp01(pushBackDistance - distToRightBound);
        
        float velocity = dir * Time.deltaTime * speed;

        transform.position += new Vector3(velocity, 0f, 0f);
        afterMove.Invoke();
    }
}
