using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Anim
{
    [SerializeField] [Tooltip("Each curve represents an axis")]private List<AnimationCurve> posCurves = new List<AnimationCurve>(3);
    [SerializeField] [Tooltip("Each curve represents an axis")]private List<AnimationCurve> rotCurves = new List<AnimationCurve>(3);
    [SerializeField] private Vector3 posMultiplier = Vector3.one;
    [SerializeField] private Vector3 rotMultiplier = Vector3.one;
    [SerializeField] public float time = 1f;
        
    [NonSerialized] public bool playing;
    [NonSerialized] public float timer;
    [NonSerialized] public Vector3 pt;
    [NonSerialized] public Vector3 rt;
        
    public IEnumerator Animate()
    {
        playing = true;
        timer = 0f;
        while (timer < time)
        {
            timer += Time.deltaTime;

            int i = 0;
            foreach (AnimationCurve curve in posCurves)
            {
                pt[i] = curve.Evaluate(Mathf.Clamp01(timer / time)) * posMultiplier[i];
                i++;
            }
            i = 0;
            foreach (AnimationCurve curve in rotCurves)
            {
                rt[i] = curve.Evaluate(Mathf.Clamp01(timer / time)) * rotMultiplier[i];
                i++;
            }
                
            yield return null;
        }
        playing = false;
    }
}