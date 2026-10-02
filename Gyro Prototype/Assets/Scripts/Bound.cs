using UnityEngine;

public class Bound : MonoBehaviour
{
    [SerializeField] private PlayerMovement player;
    [SerializeField] private bool negative = false;
    [SerializeField] private float distance;
    [SerializeField] private float power;

    private Material _mat;

    private void Awake()
    {
        _mat = GetComponent<SpriteRenderer>().material;
    }

    private void Update()
    {
        float dist = player.bounds - (negative ? -player.transform.position.x : player.transform.position.x);
        float a = Mathf.Clamp01(distance - Mathf.Pow(dist, power));
        
        _mat.SetFloat("_Alpha", a);

    }
}
