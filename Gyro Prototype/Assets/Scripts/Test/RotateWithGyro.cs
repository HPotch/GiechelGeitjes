using UnityEngine;

public class RotateWithGyro : MonoBehaviour
{
    private enum FollowGyro {gyro1, gyro2}
    [SerializeField] private FollowGyro followGyro;
    [SerializeField] private int group = 1;

    private GyroManager _gr;
    
    private void Start()
    {
        _gr = GyroManager.Instance;
        if (followGyro == FollowGyro.gyro1) _gr.onGyro1Changes[group].AddListener(SetPosition);
        if (followGyro == FollowGyro.gyro2) _gr.onGyro2Changes[group].AddListener(SetPosition);
    }

    private void SetPosition()
    {
        transform.eulerAngles = followGyro == FollowGyro.gyro1 ? _gr.gyro1Values[group] :  _gr.gyro2Values[group];
    }
}
