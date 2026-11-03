using UnityEngine;

public class LoopingPositionAnim : MonoBehaviour
{
    [SerializeField] private AnimationCurve xCurve, yCurve, zCurve;
    [SerializeField] private Vector2 randomTime = new Vector2(1f,1f);
    [SerializeField] private Vector3 amount;

    public Vector3 startPos;
    private float _time = 0f;
    private float _timer = 0f;

    private void Awake()
    {
        startPos = transform.position;
        _time = Random.Range(randomTime.x, randomTime.y);
    }

    private void Update()
    {
        _timer += Time.deltaTime;
        if (_timer > _time) _timer -= _time;
        float i = _timer / _time;
        float tx = xCurve.Evaluate(i);
        float ty = yCurve.Evaluate(i);
        float tz = zCurve.Evaluate(i);

        transform.position = new Vector3(
            Mathf.Lerp(startPos.x, startPos.x + amount.x, tx),
            Mathf.Lerp(startPos.y, startPos.y + amount.y, ty),
            Mathf.Lerp(startPos.z, startPos.z + amount.z, tz));
    }
}
