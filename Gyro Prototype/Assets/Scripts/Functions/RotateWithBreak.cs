using System;
using UnityEngine;

public class RotateWithBreak : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerMovement player;
    
    [Header("Settings")]
    [SerializeField] private Vector3 breakRotation;
    [SerializeField] private float smoothness = 10f;
    
    private Vector3 _startRot = Vector3.zero;

    private float _t = 0f;

    private void Awake()
    {
        _startRot = transform.localEulerAngles;
    }

    private void Update()
    {
        _t = Mathf.Lerp(_t, player.breaking ? 1f : 0f, Time.deltaTime * smoothness);
        transform.localEulerAngles = Vector3.Lerp(_startRot, breakRotation, _t);
    }
}
