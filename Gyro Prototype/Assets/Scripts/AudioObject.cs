using UnityEngine;
using System;

public class AudioObject : MonoBehaviour
{
    // Public variables
    [NonSerialized] public float startTime = 0f; // The time at which the playback starts
    [NonSerialized] public float endLoopTime = 0f;
    [NonSerialized] public float startFadeTime = 0f; // Fades are just linear, for now
    [NonSerialized] public float endFadeTime = 0f; // Fades are just linear, for now

    // Private variable
    private float _time = 0f; // The length of the clip
    private float _currentTime = 0f;
    private float _targetVolume = 0f;
    private bool _instancedLoop = false;
    
    // References
    private AudioSource _source;

    public void Stop(bool immediately = false)
    {
        if (immediately) Delete();
        
        _instancedLoop = true; // Stop instancing loops after stopping
        _currentTime = Mathf.Max(_currentTime, _source.clip.length - endFadeTime); // Jump to the fade
    }
    
    private void Awake()
    {
        _source = GetComponent<AudioSource>();
    }

    private void Start()
    {
        _source.Play();
        _time = _source.clip.length - startTime;
        _targetVolume = _source.volume;
        _source.time = startTime;
        
        // Failsafe
        // We prioritize the endFade over the startFade. This means the endFade will overwrite the startFade when overlapping
        // Fades are completed though, so no sudden jumps in volume will occur
        if (endLoopTime > _time)
        {
            Debug.LogWarning("Tried to loop a clip with a time longer than its duration, loopEndtime will be set to time");
            endLoopTime = _time;
        }
        if (endLoopTime > _time)
        {
            Debug.LogWarning("Tried to fade out a clip with a time longer than its duration, endFadeTime will be set to time");
            endFadeTime = _time;
        }
        if (startFadeTime > _time - endFadeTime)
        {
            Debug.LogWarning("Tried to fade in a clip with a time longer than its duration, startFadeTime will be set to time minus endFadeTime");
            startFadeTime = _time - endFadeTime;
        }
    }

    private void Update()
    {
        if (_source.loop) return;
        _currentTime += Time.deltaTime;
        
        // Startfade
        if (_currentTime < startFadeTime)
            _source.volume = Mathf.Lerp(0f, _targetVolume, _currentTime / startFadeTime);
        else _source.volume = _targetVolume;
        
        if (_currentTime > _time - endFadeTime)
        {
            // EndFade
            _source.volume = Mathf.Lerp(_targetVolume, 0f, (_currentTime - endFadeTime) / endFadeTime);
            
            // Instantiate endLoop
            if (endLoopTime > 0f && !_instancedLoop)
            {
                _instancedLoop = true;
                AudioObject audio = AudioManager.Instance.PlayAudio(_source.clip, _targetVolume, _source.pitch, endFadeTime, endFadeTime, endLoopTime, _source.panStereo);
                audio.startTime = _source.clip.length - endLoopTime;
            }
        }
        
        // End of sound
        if (_currentTime > _time) Delete();
    }
    
    private void Delete()
    {
        AudioManager.Instance.audioObjects[_source.clip].Remove(this);
        Destroy(gameObject);
    }
}
