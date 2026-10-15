// Okay so this is a receiver script, whenever the port on this script and on the gyroSender script match, this one gets the data!
// I use Euler angles, not Quaternions (I know, I know, they're inherently flawed).
// My local AI helped me a great deal with the receiving part, it's quite involved. I do understand how it works, though
// In the comments, I will treat the receiving data as mail, I honestly think it is the better name.
// I later added multiple player support, it is a bit scuffed, but essentially I made everything into lists and I just use the index to determine what group of players the data is for.

using System;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine.Events;

public class GyroManager : MonoBehaviour
{
    // Manager
    public static GyroManager Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    [Header("Settings")]
    [SerializeField] private bool sendDebugMessages = false;
    [SerializeField] private List<int> receivePorts = new List<int>();
    [Header("Events")]
    public List<UnityEvent> onGyro1Changes = new List<UnityEvent>();
    public List<UnityEvent> onGyro2Changes = new List<UnityEvent>();
    [Header("Output")]
    public List<Vector3> gyro1Values = new List<Vector3>();
    public List<Vector3> gyro1Changes = new List<Vector3>();
    public List<Vector3> gyro2Values = new List<Vector3>();
    public List<Vector3> gyro2Changes = new List<Vector3>();

    private readonly List<UdpClient> _udpClients = new List<UdpClient>();
    private List<IPEndPoint> _remoteEndPoints = new List<IPEndPoint>();
    private readonly HashSet<string> _knownPhones = new HashSet<string>();

    private void Start()
    {
        // Setup Lists
        for (int i = 0; i < receivePorts.Count; i++)
        {
            _udpClients.Add(new UdpClient(receivePorts[i]));
            Log($"UDP can send mail to {receivePorts[i]}, the receiver is checking their mail every frame (they are a little lonely, don't mind them)");
            
            _remoteEndPoints.Add(new IPEndPoint(IPAddress.Any, 0));
            
            onGyro1Changes.Add(new UnityEvent());
            onGyro2Changes.Add(new UnityEvent());
            gyro1Values.Add(Vector3.zero);
            gyro2Values.Add(Vector3.zero);
            gyro1Changes.Add(Vector3.zero);
            gyro2Changes.Add(Vector3.zero);
        }
        
        TestManager.Instance?.onRecalibrate.AddListener(Recalibrate);
    }


    private void Update()
    {
        for (int i = 0; i < receivePorts.Count; i++) GetGyroValues(i);
    }

    private void GetGyroValues(int group)
    {
        // Loop through all the mail
        while (_udpClients[group] != null && _udpClients[group].Available > 0) 
        {
            // Gather a message in a strange foreign language called bytes
            IPEndPoint remoteEndPoint = _remoteEndPoints[group];
            byte[] data = _udpClients[group].Receive(ref remoteEndPoint);
            string message = Encoding.UTF8.GetString(data); // Translate the bytes to readable text
            Log($"*MAIL ALERT* We got mail, it is from {remoteEndPoint.Address}:{remoteEndPoint.Port}, and it says: '{message}'");

            // Split the message into the correct parts
            string[] parts = message.Split(',');
            if (parts.Length >= 4)
            {
                // Detect new devices
                string phoneID = parts[0];
                if (_knownPhones.Add(phoneID))
                    Log($"<color=green>*DEVICE ALERT*</color> '{phoneID}' wants to send us some mail, how thoughtful! They're on IP: {remoteEndPoint.Address}");

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
                            gyro1Values[0] += rotation;
                            gyro1Changes[0] = rotation;
                            onGyro1Changes[group].Invoke(); break;
                        case "2":
                            gyro2Values[0] += rotation;
                            gyro2Changes[0] = rotation;
                            onGyro2Changes[group].Invoke(); break;
                        case "3":
                            gyro1Values[1] += rotation;
                            gyro1Changes[1] = rotation;
                            onGyro1Changes[group].Invoke(); break;
                        case "4":
                            gyro2Values[1] += rotation;
                            gyro2Changes[1] = rotation;
                            onGyro2Changes[group].Invoke(); break;
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
       for (int i = 0; i < _udpClients.Count; i++) _udpClients[i]?.Close();
    }

    private void Log(String text)
    {
        if (sendDebugMessages) Debug.Log(text);
    }

    private void Recalibrate()
    {
        for (int i = 0; i < gyro1Values.Count; i++) gyro1Values[i] = Vector3.zero;
        for (int i = 0; i < gyro2Values.Count; i++) gyro2Values[i] = Vector3.zero;
    }
}