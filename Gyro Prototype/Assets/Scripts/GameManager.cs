using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Events")]
    public UnityEvent onRecalibrate;
    public UnityEvent onReset;
    
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Update()
    {
        if (!Keyboard.current.rKey.wasPressedThisFrame) return;
        if(Keyboard.current.shiftKey.isPressed)
            onReset.Invoke();
        else
            onRecalibrate.Invoke();
    }
}
