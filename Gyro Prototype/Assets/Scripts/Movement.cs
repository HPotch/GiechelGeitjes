using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    private enum MoveMode { viking, goat }

    private enum GoatPhoneMode { vertical, horizontal }
    
    [Header("Viking")]
    [SerializeField] private float vikingMotionRequired = 10f; // In degrees
    [Tooltip("The viking's gyro must go from top to bottom in this time for a move to occur")] [SerializeField] private float vikingSpeedRequired = 0.7f; // In seconds
    [SerializeField] private float vikingForce = 0.1f;
    [SerializeField] private float vikingTimeInfluence = 5f;
    [SerializeField] private float velocityDrag = 5f;
    [Header("Goat")]
    [SerializeField] private float goatRotateSensitivity = 3f;
    [SerializeField] private GoatPhoneMode goatPhoneMode;
    [Header("Phone Functions")]
    [SerializeField] private MoveMode phone1Mode;
    [SerializeField] private MoveMode phone2Mode;
    [SerializeField] private int group = 1;

    private float _velocity = 0f;
    private float _direction = 0f;
    private readonly Dictionary<float, float> _lastVikingXRotations = new Dictionary<float, float>();

    private void Start()
    {
        GameManager.Instance.onReset.AddListener(Reset);
    }

    private void Update()
    {
        GyroReceiver gr = GyroReceiver.Instance;
        VikingMove(phone1Mode == MoveMode.viking ? gr.gyro1Values[group]  : gr.gyro2Values[group]);
        GoatMove(phone1Mode == MoveMode.goat ? gr.gyro1Values[group]  : gr.gyro2Values[group]);
        
        _velocity -= _velocity * velocityDrag * Time.deltaTime;
        
        transform.eulerAngles = new Vector3(0, 0, _direction);
        float dirInRadians = Mathf.Deg2Rad * _direction;
        Vector3 directionVector = new Vector3(Mathf.Cos(dirInRadians), Mathf.Sin(dirInRadians), 0f);
        transform.position +=  directionVector * _velocity;
        
        if (Keyboard.current.spaceKey.wasPressedThisFrame) MoveForward(vikingForce);
    }

    private void VikingMove(Vector3 dir)
    {
        _lastVikingXRotations.Add(Time.time, dir.x);

        float highestValue = -Mathf.Infinity;
        float highestValueTime = 0f;
        for (int i = 0; i < _lastVikingXRotations.Count; i++)
        {
            // Remove pairs that have expired
            float time = _lastVikingXRotations.Keys.ToArray()[i];
            if (time < Time.time - vikingSpeedRequired)
            {
                _lastVikingXRotations.Remove(time);
                continue;
            }
            // Get highest value
            float value = _lastVikingXRotations[time];
            if (value <= highestValue) continue;
            highestValue = value;
            highestValueTime = time;
        }

        if (!(dir.x < highestValue - vikingMotionRequired)) return;
        // The gyroscope has moved down the sufficient amount to initiate a move
        float force = vikingForce * Mathf.Lerp(vikingTimeInfluence, 1f, (Time.time - highestValueTime) / vikingSpeedRequired);
        print("Delta: " + (Time.time - highestValueTime));
        print("T: " + ((Time.time - highestValueTime) / vikingSpeedRequired));
        print("SpeedInfluence: " + Mathf.Lerp(vikingTimeInfluence, 1f, (Time.time - highestValueTime) / vikingSpeedRequired));
        MoveForward(force);
        _lastVikingXRotations.Clear(); // Clear the list so we don't initiate another move on te same values
    }

    private void GoatMove(Vector3 dir)
    {
        _direction = (dir.z - (goatPhoneMode == GoatPhoneMode.horizontal ? 45f : 0f)) * goatRotateSensitivity;
    }

    private void MoveForward(float force)
    {
        _velocity = force;
    }
    
    private void Reset()
    {
        _velocity = 0f;
        transform.position = Vector3.zero;
    }
}
