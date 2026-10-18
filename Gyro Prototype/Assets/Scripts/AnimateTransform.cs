using UnityEngine;

public class AnimateTransform : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerMovement playerMovement;
    
    public float multiplier = 1f;
    [SerializeField] private AnimationCurve curve;
    [SerializeField] private float speed;
    [SerializeField] private bool influencedBySpeed = true;
    [SerializeField] private Vector3 rotateAmount;
    [SerializeField] private Vector3 rotateOffset;
    [SerializeField] private Vector3 positionAmount;
    [SerializeField] private Vector3 positionOffset;
    [SerializeField] private Vector3 scaleAmount;
    [SerializeField] private Vector3 scaleOffset;

    private float _timer = 0f;

    private void Update()
    {
        _timer += Time.deltaTime;
        if (_timer > speed) _timer %= speed;

        float finalMultiplier = multiplier * (influencedBySpeed ? playerMovement.moveAmount / GameManager.Instance.maxSpeed : 1f);
        
        transform.localEulerAngles = LerpWithOffset(Vector3.zero, rotateAmount * 2f - rotateAmount, rotateOffset) * finalMultiplier;
        transform.localPosition = LerpWithOffset(Vector3.zero, positionAmount * 2f - positionAmount, positionOffset) * finalMultiplier;
        transform.localScale = LerpWithOffset(Vector3.one, Vector3.one + ((scaleAmount * 2f - scaleAmount) * finalMultiplier), scaleOffset) ;
    }

    private Vector3 LerpWithOffset(Vector3 start, Vector3 end, Vector3 offset)
    {
        float x = curve.Evaluate(Mathf.Repeat(_timer + offset.x, speed) / speed);
        float y = curve.Evaluate(Mathf.Repeat(_timer + offset.y, speed) / speed);
        float z = curve.Evaluate(Mathf.Repeat(_timer + offset.z, speed) / speed);

        return new Vector3(
            Mathf.Lerp(start.x, end.x, x),
            Mathf.Lerp(start.y, end.y, y),
            Mathf.Lerp(start.z, end.z, z));
    }
}
