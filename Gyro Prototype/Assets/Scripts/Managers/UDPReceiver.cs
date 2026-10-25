// Okay so this is a receiver script, whenever the port on this script and on the UDPSender script match, this one gets the data!
// This script uses Lists of floats as data
// In the comments, I will treat the receiving data as mail, I honestly think it is the better name.
// I later added multiple player support, it is a bit scuffed, but essentially I made everything into lists and I just use the index to determine what group of players the data is for.

using System;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine.Events;

public class UDPReceiver : MonoBehaviour
{
    // Manager
    public static UDPReceiver Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    
    
    [Serializable]
    public class Values {
        public List<float> values;
    }


    [Header("Settings")]
    [SerializeField] private bool sendDebugMessages = false;
    [SerializeField] private int receivePort = 12345;
    [SerializeField] private int dataAmount = 2;
    
    [Header("Events")]
    public List<UnityEvent> onFunc1Change = new List<UnityEvent>();
    public List<UnityEvent> onFunc2Change = new List<UnityEvent>();
    
    [Header("Output")]
    public List<Values> func1Values = new List<Values>();
    public List<Values> func2Values = new List<Values>();

    private readonly List<UdpClient> _udpClients = new List<UdpClient>();
    private List<IPEndPoint> _remoteEndPoints = new List<IPEndPoint>();
    private readonly HashSet<string> _knownPhones = new HashSet<string>();

    private void Start()
    {
        // Setup Lists
        for (int i = 0; i < 2; i++)
        {
            _udpClients.Add(new UdpClient(receivePort));
            Log($"UDP can send mail to {receivePort}, the receiver is checking their mail every frame (they are a little lonely, don't mind them)");
            
            _remoteEndPoints.Add(new IPEndPoint(IPAddress.Any, 0));
            
            onFunc1Change.Add(new UnityEvent());
            onFunc2Change.Add(new UnityEvent());
            
            
            var tempValues = new List<float>(); 
            for (int j = 0; j < dataAmount; j++) { tempValues.Add(0f); }

            func1Values.Add(new Values { values = tempValues.ToList()});
            func2Values.Add(new Values{values = tempValues.ToList()});
        }
        
        
        TestManager.Instance?.onRecalibrate.AddListener(Recalibrate);
    }

    private void Update()
    {
        for (int i = 0; i < 2; i++) GetAttitudeValues(i);
    }

    private void GetAttitudeValues(int group)
    {
        if (_udpClients.Count < 2) return;

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
            if (parts.Length >= dataAmount)
            {
                // Detect new devices
                string phoneID = parts[0];
                if (_knownPhones.Add(phoneID))
                    Log($"<color=green>*DEVICE ALERT*</color> '{phoneID}' wants to send us some mail, how thoughtful! They're on IP: {remoteEndPoint.Address}");

                // Some letters are written in a different culture where points are commas and commas are points, we gotta use InvariantCulture!
                bool couldParse = true;
                List<float> values = new List<float>();
                for (int i = 0; i < dataAmount; i++)
                {
                    if (float.TryParse(parts[i + 1], NumberStyles.Any, CultureInfo.InvariantCulture, out float value))
                    {
                        values.Add(value);
                        continue;
                    }
                    couldParse = false;
                    break;
                }
                
                if (couldParse && values.Count == dataAmount)
                {
                    switch (phoneID)
                    {
                        case "1":
                            func1Values[0].values = values;
                            onFunc1Change[group].Invoke(); break;
                        case "2":
                            func2Values[0].values = values;
                            onFunc2Change[group].Invoke(); break;
                        case "3":
                            func1Values[1].values = values;
                            onFunc1Change[group].Invoke(); break;
                        case "4":
                            func2Values[1].values = values;
                            onFunc2Change[group].Invoke(); break;
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
        func1Values.Clear();
        func2Values.Clear();
        func1Values.Add(new Values{values = new List<float>(dataAmount)});
        func2Values.Add(new Values{values = new List<float>(dataAmount)});
    }
}