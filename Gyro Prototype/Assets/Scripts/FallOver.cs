using System;
using System.Collections;
using UnityEngine;

public class FallOver : MonoBehaviour
{
    [Serializable]
    public enum Axis
    { x,y,z }
    
    [SerializeField] private float fallTime = 0.1f;
    [SerializeField] private Axis axis;
    
    
    public void Execute()
    {
        StartCoroutine(FallOverRoutine());
    }

    private IEnumerator FallOverRoutine()
    {
        float timer = 0f;

        while (timer < fallTime)
        {
            timer += Time.deltaTime;
            float r = Mathf.Lerp(0, 90, timer / fallTime);
            transform.eulerAngles = new Vector3(
            axis == Axis.x ? r : 0f,
            axis == Axis.y ? r : 0f,
            axis == Axis.z ? r : 0f);
            
            yield return null;
        }
        
        transform.eulerAngles = new Vector3(
            axis == Axis.x ? 90f : 0f,
            axis == Axis.y ? 90f : 0f,
            axis == Axis.z ? 90f : 0f);
    }
}
