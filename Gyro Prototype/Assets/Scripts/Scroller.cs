using UnityEngine;

public class Scroller : MonoBehaviour
{
    [SerializeField] private float modPosition = 1f;
    
    private void Update()
    {
        transform.localPosition = new Vector3(
            Mathf.Repeat(transform.localPosition.x, modPosition),
            Mathf.Repeat(transform.localPosition.y, modPosition),
            Mathf.Repeat(transform.localPosition.z, modPosition));
    }
}
