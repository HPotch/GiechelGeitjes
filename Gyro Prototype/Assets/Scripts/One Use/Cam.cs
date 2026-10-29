using UnityEngine;

public class Cam : MonoBehaviour
{
    public float additionalFov = 0f;
    
    [SerializeField] private PlayerMovement player;
    [SerializeField] private Vector2 fovRange = new Vector2(70, 100);
    [SerializeField] private float smoothness = 20f;

    private Camera _cam;

    private void Awake()
    {
        _cam = GetComponent<Camera>();
    }

    private void Update()
    {
        float range = player.moveAmount / GameManager.Instance.maxSpeed;
        float target = Mathf.Lerp(fovRange.x, fovRange.y, range) + additionalFov;
        _cam.fieldOfView = Mathf.Lerp(_cam.fieldOfView, target, smoothness * Time.deltaTime);
    }
}
