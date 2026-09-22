// Okay so this is a receiver script, whenever the port on this script and on the gyroSender script match, this one gets the data!
// I use Euler angles, not Quaternions (I know, I know, they're inherently flawed).
// My local AI helped me a great deal with the receiving part, it's quite involved. I do understand how it works, though
// In the comments, I will treat the receiving data as mail, I honestly think it is the better name.

using System;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine.Events;

public class GyroReceiver : MonoBehaviour
{
    // Manager
    public static GyroReceiver Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }


    [Header("Settings")]
    [SerializeField] private bool sendDebugMessages = false;
    [SerializeField] private int receivePort = 12345;
    [Header("Events")]
    public UnityEvent onGyro1Change;
    public UnityEvent onGyro2Change;
    [Header("Output")]
    public Vector3 gyro1Value;
    public Vector3 gyro2Value;

    private UdpClient _udpClient;
    private IPEndPoint _remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
    private readonly HashSet<string> _knownPhones = new HashSet<string>();
    private float _lastPacketTime = 0f;

    private void Start()
    {
        _udpClient = new UdpClient(receivePort);
        Log($"UDP can send mail to {receivePort}, the receiver is checking their mail every frame (they are a little lonely, don't mind them)");
        GameManager.Instance.onRecalibrate.AddListener(Recalibrate);
    }

    private void Update()
    {
        // Loop through all the mail
        while (_udpClient != null && _udpClient.Available > 0) 
        {
            // Gather a message in a strange foreign language called bytes
            byte[] data = _udpClient.Receive(ref _remoteEndPoint);
            string message = Encoding.UTF8.GetString(data); // Translate the bytes to readable text
            Log($"*MAIL ALERT* We got mail, it is from {_remoteEndPoint.Address}:{_remoteEndPoint.Port}, and it says: '{message}'");

            // Split the message into the correct parts
            string[] parts = message.Split(',');
            if (parts.Length >= 4)
            {
                // Detect new devices
                string phoneID = parts[0];
                if (_knownPhones.Add(phoneID))
                    Log($"<color=green>*DEVICE ALERT*</color> '{phoneID}' wants to send us some mail, how thoughtful! They're on IP: {_remoteEndPoint.Address}");

                // Some letters are written in a different culture where points are commas and commas are points, we gotta use InvariantCulture!
                if (float.TryParse(parts[1], NumberStyles.Any, CultureInfo.InvariantCulture, out float x) &&
                    float.TryParse(parts[2], NumberStyles.Any, CultureInfo.InvariantCulture, out float y) &&
                    float.TryParse(parts[3], NumberStyles.Any, CultureInfo.InvariantCulture, out float z))
                {
                    Vector3 rotation = new Vector3(x, y, z);

                    if (rotation == Vector3.zero) continue;
                    switch (phoneID)
                    {
                        case "1":
                            gyro1Value += rotation;
                            onGyro1Change.Invoke(); break;
                        case "2":
                            gyro2Value += rotation;
                            onGyro2Change.Invoke(); break;
                    }
                }
                else
                    Debug.LogWarning($"*PARSE ALERT* The receiver couldn't do anything with the message: '{message}'");
            }
            else
                Debug.LogWarning($"*MESSAGE LENGTH ALERT* The receiver expects messages from at least 4 parts, but he got {parts.Length} from the message: '{message}'");
        }
    }

    private void OnApplicationQuit()
    {
        _udpClient?.Close();
    }

    private void Log(String text)
    {
        if (sendDebugMessages) Debug.Log(text);
    }

    private void Recalibrate()
    {
        gyro1Value = Vector3.zero;
        gyro2Value = Vector3.zero;
    }
}