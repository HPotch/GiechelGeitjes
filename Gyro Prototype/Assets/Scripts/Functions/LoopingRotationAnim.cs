using UnityEngine;

public class LoopingRotation : MonoBehaviour
{
    [SerializeField] private AnimationCurve xCurve, yCurve, zCurve;
    [SerializeField] private Vector2 randomTime = new Vector2(1f,1f);
    [SerializeField] private Vector3 amount;

    public Vector3 startRot;
    public bool enabled = true;
    private float _time = 0f;
    private float _timer = 0f;

    private void Awake()
    {
        startRot = transform.localEulerAngles;
        _time = Random.Range(randomTime.x, randomTime.y);
    }

    private void Update()
    {
        if (enabled)
            Animate();
        else
            transform.localEulerAngles = Vector3.Lerp(transform.localEulerAngles, startRot, 3 * Time.deltaTime);
        
    }

    private void Animate()
    {
        _timer += Time.deltaTime;
        if (_timer > _time) _timer -= _time;
        float i = _timer / _time;
        float tx = xCurve.Evaluate(i);
        float ty = yCurve.Evaluate(i);
        float tz = zCurve.Evaluate(i);

        transform.localEulerAngles = new Vector3(
            Mathf.Lerp(startRot.x, startRot.x + amount.x, tx),
            Mathf.Lerp(startRot.y, startRot.y + amount.y, ty),
            Mathf.Lerp(startRot.z, startRot.z + amount.z, tz));
    }
}
