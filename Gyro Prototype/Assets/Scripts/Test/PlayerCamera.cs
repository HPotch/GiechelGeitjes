using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private Transform follow;
    
    private void Update()
    {
        transform.position = Vector2.Lerp(transform.position, follow.position, 20f * Time.deltaTime);
        transform.position =  new Vector3(transform.position.x, transform.position.y, -10);
    }
}
