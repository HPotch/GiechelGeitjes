using UnityEngine;

public class RotateWithGyro : MonoBehaviour
{
    private enum FollowGyro {gyro1, gyro2}
    [SerializeField] private FollowGyro followGyro;

    private GyroReceiver _gr;
    
    private void Start()
    {
        _gr = GyroReceiver.Instance;
        if (followGyro == FollowGyro.gyro1) _gr.onGyro1Change.AddListener(SetPosition);
        if (followGyro == FollowGyro.gyro2) _gr.onGyro2Change.AddListener(SetPosition);
    }

    private void SetPosition()
    {
        transform.eulerAngles = followGyro == FollowGyro.gyro1 ? _gr.gyro1Value :  _gr.gyro2Value;
    }
}
