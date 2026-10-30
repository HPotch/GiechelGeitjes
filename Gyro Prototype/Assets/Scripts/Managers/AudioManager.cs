using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

public class AudioManager : MonoBehaviour
{
    // Manager
    public static AudioManager Instance;
    private void Awake()
    {
        if (!Instance) Instance = this;
        else Destroy(gameObject);
    }
    
    // Public variables
    [NonSerialized] public  Dictionary<AudioClip, List<AudioObject>> audioObjects = new Dictionary<AudioClip, List<AudioObject>>();
    
    // Public classes
    [Serializable]
    public class AudioSettings
    {
        public AudioClip[] clips;
        public float volume = 1, pitch = 1f;
        public float randomVolume, randomPitch, startFade, endFade, endLoop, pan;
        public bool loop;
        public AudioMixerGroup mixerGroup; 
    }
    
    // Editor references
    [SerializeField] private GameObject audioObject;
    
    /// <summary>
    /// <param name="startFade">The duration of a linear fade in</param>
    /// <param name="endFade">The duration of a linear fade out</param>
    /// <param name="endLoop">The duration of the last part of the sound that will be looped indefinitely</param>
    /// </summary>
    public AudioObject PlayAudio(AudioClip clip, float volume = 1f, float pitch = 1f, float startFade = 0f, float endFade = 0f, float endLoop = 0f, float pan = 0f, bool loop = false, AudioMixerGroup mixerGroup = null)
    {
        // Instantiate
        GameObject instance = Instantiate(audioObject, transform);
        AudioObject audio = instance.GetComponent<AudioObject>();
        AudioSource source = instance.GetComponent<AudioSource>();

        audio.endLoopTime = endLoop;
        audio.startFadeTime = startFade;
        audio.endFadeTime = loop ? 0f : endFade; // Disable endFade if looping
        source.clip = clip;
        source.volume = volume;
        source.pitch = pitch;
        source.panStereo = pan;
        source.loop = loop && endLoop == 0f; // Disable loop if endLooping
        source.outputAudioMixerGroup = mixerGroup;
        
        // Add to dictionary
        audio.gameObject.name = clip.name; // Mainly debugging and ocd lol
        audioObjects.TryGetValue(clip, out List<AudioObject> audioList);
        if  (audioList == null) audioList = new List<AudioObject>();
        audioList.Add(audio);
        audioObjects[clip] = audioList;
        
        // Return
        return audio;
    }

    public AudioObject PlayRandomAudio(AudioClip[] clips, float volume = 1f, float pitch = 1f, float startFade = 0f, float endFade = 0f, float endLoop = 0f, float pan = 0f, bool loop = false, AudioMixerGroup mixerGroup = null)
    {
        return PlayAudio(clips[Random.Range(0, clips.Length)], volume, pitch, startFade, endFade, endLoop, pan, loop, mixerGroup);
    }
    
    public AudioObject PlayAudioSettings(AudioSettings a)
    {
        if (a.clips.Length == 0)
        {
            Debug.LogError($"No clips passed in audioSettings {a}");
            return null;
        }
        return PlayAudio(a.clips[Random.Range(0, a.clips.Length)], a.volume + Random.Range(-a.randomVolume, a.randomVolume), a.pitch + Random.Range(-a.randomPitch, a.randomPitch), a.startFade, a.endFade, a.endLoop, a.pan, a.loop, a.mixerGroup);
    }

    public AudioObject GetFirstAudio(AudioClip clip)
    {
        if (!audioObjects.ContainsKey(clip)) return null;
        if (audioObjects[clip].Count == 0) return null;
        return audioObjects[clip][0];
    }
    
    public List<AudioObject> GetAllAudio(AudioClip clip)
    {
        if (!audioObjects.ContainsKey(clip)) return null;
        if (audioObjects[clip].Count == 0) return null;
        return audioObjects[clip];
    }
    
    public void StopAudioObject(AudioObject audioObject)
    {
        audioObject.Stop();
    }
    
    public void StopFirstAudio(AudioClip clip)
    {
        StopAudioObject(GetFirstAudio(clip));
    }

    public void StopAllAudio(AudioClip clip)
    {
        List<AudioObject> audioToDelete = GetAllAudio(clip);
        foreach (AudioObject audio in audioToDelete)
            StopAudioObject(audio);
    }
}
