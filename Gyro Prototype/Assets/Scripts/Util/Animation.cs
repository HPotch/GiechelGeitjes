using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class Animation : MonoBehaviour
{
    [SerializeField] private AnimationCurve curve;
    [SerializeField] private float multiplier = 1f;
    [SerializeField] private float time = 1f;
    public bool playing;

    public float timer;
    public float t;
        
    public void StartAnimation()
    {
        playing = true;
        StartCoroutine(Animate());
    }
        
    private IEnumerator Animate()
    {
        timer = 0f;
        while (timer < time)
        {
            timer += Time.deltaTime / time;
            t = curve.Evaluate(Mathf.Clamp01(timer / time) * multiplier);
            yield return null;
        }
        playing = false;
    }
}