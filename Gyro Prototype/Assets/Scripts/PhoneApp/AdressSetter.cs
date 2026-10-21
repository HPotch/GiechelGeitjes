using System;
using TMPro;
using UnityEngine;

public class AdressSetter : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float warningTime = 3f;
    [Header("References")]
    [SerializeField] private TMP_InputField IPInput;
    [SerializeField] private TMP_InputField portInput;
    [SerializeField] private URPSender urps;
    [SerializeField] private TextMeshProUGUI text;

    private void Awake()
    {
        if (!urps) urps = GetComponent<URPSender>();
    }

    private void Start()
    {
        PlayerPrefs.DeleteAll();
    }

    public void SetIP()
    {
        string[] segments = IPInput.text.Split('.');

        if (segments.Length != 4)
        {
            SetMessage("IP must be in the format X.X.X.X", Color.red);
            return;
        }

        foreach (var s in segments)
        {
            if (s.Length == 0)
            {
                SetMessage("IP segments cannot be empty", Color.red);
                return;
            }
            if (!int.TryParse(s, out int segmentValue))
            {
                SetMessage("IP segments cannot contain letters", Color.red);
                return;
            }
            if (segmentValue < 0 || segmentValue > 255)
            {
                SetMessage("All IP segments should be between 0 and 255", Color.red);
                return;
            }
            if (s.Length > 1 && s.StartsWith("0"))
            {
                SetMessage("IP segments cannot start with a zero", Color.red);
                return;
            }
        }
        SetMessage("IP is valid!", Color.green);
        urps.serverIP = IPInput.text;
        
        //PlayerPrefs.SetString("IP", gyroSender.serverIP);
    }
    
    // public void SetPort()
    // {
    //     if (!int.TryParse(portInput.text.Trim(), out var port))
    //     {
    //         SetMessage("Port should be a number", Color.red);
    //         return;
    //     }
    //     if (port < 1024 || port > 65535)
    //     {
    //         SetMessage("Port should be between 1024 and 65535", Color.red);
    //         return;
    //     }
    //     SetMessage("Port is valid!", Color.green);
    //     gyroSender.port = port;
    //     PlayerPrefs.SetInt("Port", gyroSender.port);
    // }

    public void IPInfo()
    {
        SetMessage("You should input the IP of your PC. You can find it by opening the terminal and running:\nWindows: ipconfig (then look for IPv4)\nMac: ipconfig getifaddr en0 (or en1)\nLinux: hostname -I\nAn example is: 192.168.68.123", Color.white, 16f);
    }

    // public void PortInfo()
    // {
    //     SetMessage("You should match the port number on this device and your PC, as long as they match, you're good!", Color.white, 10f);
    // }

    public void SetPhone1()
    {
        urps.phoneID = "1";
    }

    public void SetPhone2()
    {
        urps.phoneID = "2";
    }
    
    public void SetPhone3()
    {
        urps.phoneID = "3";
    }

    public void SetPhone4()
    {
        urps.phoneID = "4";
    }

    private void SetMessage(String message, Color color, float waitTime = 0f)
    {
        if (waitTime == 0f) waitTime = warningTime;
        string hex = $"#{ColorUtility.ToHtmlStringRGB(color).ToUpper()}"; 
        
        text.text = $"<color={hex}>{message}</color>";
        urps.stopTextTime = waitTime;
    }
}
