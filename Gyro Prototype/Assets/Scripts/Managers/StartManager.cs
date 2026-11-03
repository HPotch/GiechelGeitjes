using System.Collections;
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

    [SerializeField] private RectTransform[] ones, twos, threes, goats;
    
    [SerializeField] private float startWait = 1f;
    [SerializeField] private AnimationCurve numberCurve, goatCurveX, goatCurveY;
    [SerializeField] private float numberHeight = 1000f, goatWidth = 1000f, goatHeight = 500f, goatTime = 2f;
    [SerializeField] private AudioSettings[] audioOnAwake, afterStartAudio;
    [SerializeField] private AudioSettings startAudio;

    private bool _executed = false;
    private float _startY, _goatStartY;
    
    private void Start()
    {
        foreach (AudioSettings a in audioOnAwake)
            AudioManager.Instance.PlayAudioSettings(a);
        
        _startY = ones[0].anchoredPosition.y;
        _goatStartY = goats[0].anchoredPosition.y;
        foreach (RectTransform rt in ones) rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, _startY - numberHeight);
        foreach (RectTransform rt in twos) rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, _startY - numberHeight);
        foreach (RectTransform rt in threes) rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, _startY - numberHeight);
        foreach (RectTransform rt in goats) rt.anchoredPosition = new Vector2(goatWidth, _goatStartY);
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
        AudioManager.Instance.PlayAudioSettings(startAudio);
        
        // Three
        float timer = 0f;
        while (timer < 1f)
        {
            timer += Time.deltaTime;
            HandleNumbers(threes, timer);
            yield return null;
        }

        // Two
        timer = 0f;
        while (timer < 1f)
        {
            timer += Time.deltaTime;
            HandleNumbers(twos, timer);
            yield return null;
        }
        
        // One
        timer = 0f;
        while (timer < 1f)
        {
            timer += Time.deltaTime;
            HandleNumbers(ones, timer);
            yield return null;
        }
        
        // GOAT!
        timer = 0f;
        while (timer < goatTime)
        {
            timer += Time.deltaTime;
            float tx = goatCurveX.Evaluate(timer / goatTime);
            float ty = goatCurveY.Evaluate(timer / goatTime);
            foreach (RectTransform rt in goats)
                rt.anchoredPosition = new Vector2(
                    Mathf.Lerp(goatWidth, -goatWidth, tx),
                    Mathf.Lerp(_goatStartY, _goatStartY + goatHeight, ty));
            
            HandleNumbers(ones, timer, true);
            HandleNumbers(twos, timer, true);
            HandleNumbers(threes, timer, true);
            
            yield return null;
        }

        GameManager.Instance.started = true;
        foreach (AudioSettings a in afterStartAudio)
            AudioManager.Instance.PlayAudioSettings(a);
    }

    private void HandleNumbers(RectTransform[] list, float timer, bool reverse = false)
    {
        float t = numberCurve.Evaluate(timer / 1f);
        foreach (RectTransform rt in list)
            rt.anchoredPosition = new Vector2(
                rt.anchoredPosition.x,
                reverse 
                ? Mathf.Lerp(_startY, _startY - numberHeight, t)
                : Mathf.Lerp(_startY - numberHeight, _startY, t));
    }
}
