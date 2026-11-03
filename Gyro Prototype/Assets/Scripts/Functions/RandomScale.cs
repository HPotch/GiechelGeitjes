using UnityEngine;
using Random = UnityEngine.Random;

public class RandomScale : MonoBehaviour
{
    [SerializeField] private Vector2 scale = new Vector2(0.5f, 2f);

    private void Awake()
    {
        transform.localScale *= Random.Range(scale.x, scale.y);
        Destroy(this);
    }
}
