using System;
using UnityEngine;
using UnityEngine.Events;

public class RoadObject : MonoBehaviour
{
    public enum Type
    {
        HighObstacle, LowObstacle, Item
    }

    [SerializeField] private Type type = Type.LowObstacle;
    [SerializeField] [Tooltip("Units per second")] private Vector3 moveDirection = new Vector3(1f, 0, 0);

    [SerializeField] private float destroyBeyond = 2f;

    public UnityEvent onExecute;
    public PlayerMovement player;
    private FallOver _fallOver;

    private void Awake()
    {
        _fallOver = GetComponent<FallOver>();
    }

    private void Update()
    {
        if (transform.position.x > GameManager.Instance.Loser().position.x)
            Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Collide(collision.collider);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        Collide(other);
    }

    private void Collide(Collider collider)
    {
        if (collider.gameObject.CompareTag("Destroyer") && type != Type.Item)
        {
            FallOver();
        }
        
        if (!collider.gameObject.CompareTag("Player")) return;
        player = collider.GetComponent<PlayerMovement>();
        switch (type)
        {
            case Type.HighObstacle or  Type.LowObstacle:
                CollideObstacle(); break;
            case Type.Item:
                CollideItem(); break;
        }
    }

    private void CollideObstacle()
    {
        
        if (type == Type.HighObstacle) CrashManager.Instance.Crash(CrashManager.CrashType.High);
        else if (type == Type.LowObstacle) CrashManager.Instance.Crash(CrashManager.CrashType.Low);
    }

    private void CollideItem()
    {
        onExecute.Invoke();
        Destroy(gameObject);
    }

    private void FallOver()
    {
        if (_fallOver) _fallOver.Execute();
    }
}
