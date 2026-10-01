using UnityEngine;
using UnityEngine.SceneManagement;

public class RoadObject : MonoBehaviour
{
    public enum Type
    {
        Obstacle, Item
    }

    [SerializeField] private Type type = Type.Obstacle;
    [SerializeField] [Tooltip("Units per second")] private Vector3 moveDirection = new Vector3(1f, 0, 0);

    [SerializeField] private float destroyBeyond = 21f;
    
    private void Update()
    {
        // Scroll
        transform.localPosition += moveDirection * (Time.deltaTime * GameManager.Instance.SpeedScale);
        
        if (transform.localPosition.x > destroyBeyond)
            Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (type)
        {
            case Type.Obstacle:
                CollideObstacle(other); break;
            case Type.Item:
                CollideItem(other); break;
        }
    }

    private void CollideObstacle(Collider other)
    {
        if (!other.gameObject.CompareTag("Player")) return;
        SceneManager.LoadScene("GameOver");
        Destroy(gameObject);
    }

    private void CollideItem(Collider other)
    {
        if (!other.gameObject.CompareTag("Player")) return;
        Debug.Log("Item collected");
        Destroy(gameObject);
    }
}
