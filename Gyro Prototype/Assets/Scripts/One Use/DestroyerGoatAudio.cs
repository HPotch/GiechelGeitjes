using UnityEngine;

public class DestroyerGoatAudio : MonoBehaviour
{
    public AudioSource hoovesSource;
    public Transform lastGoat;
    public float noSoundDistance, maxVolume;
    
    private void Update()
    {
        if (!hoovesSource) Destroy(gameObject);
        // Stop
        if (!lastGoat)
        {
            AudioManager.Instance.StopAudioObject(hoovesSource.GetComponent<AudioObject>(), true);
            return;
        }
        
        // Get closest distance
        float p1Dist = Mathf.Abs(GameManager.Instance.Leader().position.z - lastGoat.position.z);
        float p2Dist = Mathf.Abs(GameManager.Instance.Loser().position.z - lastGoat.position.z);
        float minDist = Mathf.Min(p1Dist, p2Dist);
        
        // Set volume
        float i = Mathf.Max(minDist / noSoundDistance, 0f);
        hoovesSource.volume = Mathf.Lerp(maxVolume, 0f, i);
    }
}
