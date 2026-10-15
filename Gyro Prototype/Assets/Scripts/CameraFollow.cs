using System;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Serializable]
    private enum Smoothmode
    {
        None,
        Smoothed,
        Velocity
    }
    
    [Serializable]
    public class FollowAxis
    {
        public bool x, y, z;
    }
    
    [SerializeField] private Transform target;
    [SerializeField] private FollowAxis followAxis;
    [SerializeField] private Smoothmode smoothMode = Smoothmode.None;
    [SerializeField] private float smoothness = 20f;
    
    private Vector3 _velocity;
    private Vector3 _smoothedTargetPos;
    private Vector3 _offset;

    private void Awake()
    {
        _offset = transform.position - target.position;
    }

    private void Update()
    {
        // Set target
        Vector3 p1 = transform.position, p2 = target.position + _offset;
        Vector3  targetPosition = new Vector3(
            followAxis.x ? p2.x : p1.x,
            followAxis.y ? p2.y : p1.y,
            followAxis.z? p2.z : p1.z);
        
        // Velocity
        if (smoothMode == Smoothmode.Velocity)
        {
            _velocity = Vector3.Lerp(_velocity, targetPosition - _smoothedTargetPos, smoothness * Time.deltaTime);
            _smoothedTargetPos += _velocity;
        }
        else
            _smoothedTargetPos = targetPosition;
        
        // Smooth
        if (smoothMode == Smoothmode.Smoothed)
            transform.position = Vector3.Lerp(transform.position, _smoothedTargetPos, Time.deltaTime * smoothness);
        else
            transform.position = _smoothedTargetPos; // No smooth or velocity
    }
}
