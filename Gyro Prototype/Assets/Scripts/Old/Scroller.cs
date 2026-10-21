// DONT NEED THIS SCRIPT ANYMORE

using UnityEngine;

public class Scroller : MonoBehaviour
{
    [SerializeField] [Tooltip("Units per second")] private Vector3 moveDirection = new Vector3(0, 0, -1);
    [SerializeField] private float modPosition = 1f;
    
    private void Update()
    {
        transform.localPosition += moveDirection * (Time.deltaTime); // * GameManager.Instance.moveAmount
        transform.localPosition = new Vector3(
            Mathf.Repeat(transform.localPosition.x, modPosition),
            Mathf.Repeat(transform.localPosition.y, modPosition),
            Mathf.Repeat(transform.localPosition.z, modPosition));
    }
}
