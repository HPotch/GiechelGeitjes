using UnityEngine;

public class PhoneSetter : MonoBehaviour
{
    private GyroSender _gyroSender;

    private void Awake()
    {
        _gyroSender = GetComponent<GyroSender>();
    }

    public void SetPhone1()
    {
        _gyroSender.phoneID = "1";
    }

    public void SetPhone2()
    {
        _gyroSender.phoneID = "2";
    }
}
