using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using AudioSettings = AudioManager.AudioSettings;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Events")]
    public UnityEvent onRecalibrate;
    public UnityEvent onReset;
    
    [Header("Settings")]
    [SerializeField] private AudioSettings  goatAmience;
    
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    
    void Start ()
    {
        Debug.Log("Displays connected:" + Display.displays.Length);
        for (int i = 0; i < Display.displays.Length; i++)
        {
            Display.displays[i].Activate();
        }
        AudioManager.Instance.PlayAudioSettings(goatAmience);
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
