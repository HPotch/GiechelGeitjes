using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UDPHelper : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float warningTime = 3f;
    [Header("References")]
    [SerializeField] private TMP_InputField IPInput;
    [SerializeField] private UDPSender udps;
    [SerializeField] private List<TextMeshProUGUI> debug;
    [SerializeField] private TextMeshProUGUI info;
    [SerializeField] private List<GameObject> settings;
    [SerializeField] private List<GameObject> noSettings;

    private bool _settingScreen = true;
    
    private void Awake()
    {
        if (!udps) udps = GetComponent<UDPSender>();
        SetSettingScreen(true);
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
        udps.serverIP = IPInput.text;
        
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
        udps.phoneID = "1";
        info.text = "Playing as: Goat 1";
        SetSettingScreen(false);
    }

    public void SetPhone2()
    {
        udps.phoneID = "2";
        info.text = "Playing as: Viking 1";
        SetSettingScreen(false);
    }
    
    public void SetPhone3()
    {
        udps.phoneID = "3";
        info.text = "Playing as: Goat 2";
        SetSettingScreen(false);
    }

    public void SetPhone4()
    {
        udps.phoneID = "4";
        info.text = "Playing as: Viking 2";
        SetSettingScreen(false);
    }
    
    public void SetSettingScreen(bool set)
    {
        _settingScreen = set;
        if (set)
        {
            foreach (GameObject obj in settings)
                obj.SetActive(true);
            foreach (GameObject obj in noSettings)
                obj.SetActive(false);
        }
        else
        {
            foreach (GameObject obj in settings)
                obj.SetActive(false);
            foreach (GameObject obj in noSettings)
                obj.SetActive(true);
        }
    }

    private void SetMessage(String message, Color color, float waitTime = 0f)
    {
        if (waitTime == 0f) waitTime = warningTime;
        string hex = $"#{ColorUtility.ToHtmlStringRGB(color).ToUpper()}"; 
        
        foreach (TextMeshProUGUI text in debug)
            text.text = $"<color={hex}>{message}</color>";
        udps.stopTextTime = waitTime;
    }
}
