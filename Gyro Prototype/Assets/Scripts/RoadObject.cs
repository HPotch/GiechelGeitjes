using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoadObject : MonoBehaviour
{
    public enum Type
    {
        HighObstacle, LowObstacle, Item
    }

    [SerializeField] private Type type = Type.LowObstacle;
    [SerializeField] [Tooltip("Units per second")] private Vector3 moveDirection = Vector3.back;

    [SerializeField] private float destroyBeyond = 21f;

    private void Update()
    {
        // Scroll
        transform.localPosition += moveDirection * (Time.deltaTime * GameManager.Instance.moveAmount);
        
        if (transform.localPosition.x > destroyBeyond)
            Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        switch (type)
        {
            case Type.HighObstacle or  Type.LowObstacle:
                CollideObstacle(collision.collider); break;
            case Type.Item:
                CollideItem(collision.collider); break;
        }
    }

    private void CollideObstacle(Collider other)
    {
        if (!other.gameObject.CompareTag("Player")) return;
        
        if (type == Type.HighObstacle) CrashManager.Instance.Crash(CrashManager.CrashType.High);
        else if (type == Type.LowObstacle) CrashManager.Instance.Crash(CrashManager.CrashType.Low);
    }

    private void CollideItem(Collider other)
    {
        if (!other.gameObject.CompareTag("Player")) return;
        Debug.Log("Item collected");
        Destroy(gameObject);
    }
}
