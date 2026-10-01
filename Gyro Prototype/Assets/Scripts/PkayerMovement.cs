using UnityEngine;
using UnityEngine.InputSystem;

public class PkayerMovement : MonoBehaviour
{
    [SerializeField] [Tooltip("How far the player can move")] private float bounds = 5f;
    [SerializeField] [Tooltip("Units per second")] private float speed = 2f;

    private void Update()
    {
        if (Keyboard.current.leftArrowKey.isPressed)
            transform.position += new Vector3(-speed * Time.deltaTime, 0f, 0f);
        if (Keyboard.current.rightArrowKey.isPressed)
            transform.position += new Vector3(speed * Time.deltaTime, 0f, 0f);
        transform.position =  new Vector3(Mathf.Clamp(transform.position.x, -bounds, bounds), transform.position.y, transform.position.z);
    }
}
