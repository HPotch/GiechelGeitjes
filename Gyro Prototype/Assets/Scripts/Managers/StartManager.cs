using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using AudioSettings = AudioManager.AudioSettings;


public class StartManager : MonoBehaviour
{
    public static StartManager Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    [SerializeField] private Image[] images;
    
    [SerializeField] private float startWait = 1f;
    [SerializeField] private Sprite[] sprites; // Should contain "3", "2", "1", "GO" sprites
    [SerializeField] private AnimationCurve sizeCurve;
    [SerializeField] private AudioSettings audioSettings;

    private bool _executed = false;
    
    private void Start()
    {
        foreach (Image img in images)
            img.transform.localScale = Vector3.zero;
    }
    
    public void StartGame()
    {
        if (_executed) return;
        StartCoroutine(StartRoutine());
        _executed = true;
    }

    private IEnumerator StartRoutine()
    {
        yield return new WaitForSeconds(startWait);
        
        float timer = 0f;
        for (int i = 0; i < 4; i++)
        {
            foreach (var img in images)
                img.sprite = sprites[i];

            AudioManager.Instance.PlayAudioSettings(audioSettings);
            
            while (timer < 1f)
            {
                timer += Time.deltaTime;
                float t = sizeCurve.Evaluate(timer / 1f);
                foreach (var img in images)
                    img.transform.localScale = Vector3.one * Mathf.Lerp(0f, 1f, t);

                yield return null;
            }
            
            foreach (var img in images)
                img.transform.localScale = Vector3.zero;

            timer = 0f;
        }

        GameManager.Instance.started = true;
    }
}
