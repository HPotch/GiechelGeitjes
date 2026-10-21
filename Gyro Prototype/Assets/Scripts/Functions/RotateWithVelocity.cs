using UnityEngine;
using System;

public class RotateWithVelocity : MonoBehaviour
{
    public float multiplier = 1f;
    [Serializable]
    public enum Axis {x,y,z};
    
    [SerializeField] private Axis velocityAxis = Axis.x;
    [SerializeField] private Axis rotateAxis = Axis.z;
    [SerializeField] private float smoothness = 20f;
    [SerializeField] private float rotateAmount;
    [SerializeField] private AnimationCurve amountCurve;
    [SerializeField] private float curveRange = 30f;

    private float _previousPosition;
    private float _velocity = 0f;
    private float _previousTime = 0f;
    private float _smoothedTargetRot = 0f;

    private void Awake()
    {
        _previousPosition = GetPos();
    }

    public void UpdateRotation()
    {
        float pos = GetPos();
        float deltaTime = Time.time - _previousTime;
        float velocity = deltaTime == 0 ? 0f : (pos - _previousPosition) / deltaTime;
        
        float targetRotation = velocity * rotateAmount;
        _velocity = Mathf.Lerp(_velocity, targetRotation - _smoothedTargetRot, deltaTime * smoothness);
        _smoothedTargetRot += _velocity;
        
        float finalRotation = amountCurve.Evaluate(Mathf.Abs(_smoothedTargetRot * multiplier) / curveRange) * curveRange * Mathf.Sign(_smoothedTargetRot);
        SetRot(finalRotation);
        
        _previousPosition = GetPos();
        _previousTime = Time.time;
    }

    private float GetPos()
    {
        switch (velocityAxis)
        {
            case Axis.x: return transform.position.x;
            case Axis.y: return transform.position.y;
            case Axis.z: return transform.position.z;
        }
        return 0f;
    }
    
    private void SetRot(float value)
    {
        transform.localEulerAngles = new Vector3(
            rotateAxis == Axis.x ? value : 0f,
            rotateAxis == Axis.y ? value : 0f,
            rotateAxis == Axis.z ? value : 0f);
    }
}
