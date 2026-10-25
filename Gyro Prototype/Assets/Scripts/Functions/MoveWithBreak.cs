using UnityEngine;

public class MoveWithBreak : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerMovement player;
    
    [Header("Settings")]
    [SerializeField] private Vector3 normalPos = Vector3.zero;
    [SerializeField] private Vector3 breakPos = Vector3.forward;
    [SerializeField] private float smoothness = 10f;

    private float _t = 0f;

    private void Update()
    {
        _t = Mathf.Lerp(_t, player.breaking ? 1f : 0f, Time.deltaTime * smoothness);
        transform.localPosition = Vector3.Lerp(normalPos, breakPos, _t);
    }
}
