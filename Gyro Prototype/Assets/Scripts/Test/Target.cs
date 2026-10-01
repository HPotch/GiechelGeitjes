using UnityEngine;
using AudioSettings = AudioManager.AudioSettings;

public class Target : MonoBehaviour
{
    [SerializeField] private AudioSettings  pickup;
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.transform.CompareTag("Player")) return;
        print(collision.transform.name + " won a point!");
        AudioManager.Instance.PlayAudioSettings(pickup);
        Destroy(gameObject);
    }
}
