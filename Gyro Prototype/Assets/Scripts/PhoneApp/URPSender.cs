// This is a little script that sends attitudeSensor data through the UDP-client (which uses a IP network)
// The data it sends is formatted as: "ID,x,y,y", where ID is an assignable device ID,  xy is the quaternion rotation of the attitudeSensor and y is the accelerometer's y

using System;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Text;
using TMPro;
using UnityEngine.InputSystem;
using Gyroscope = UnityEngine.InputSystem.Gyroscope;


public class URPSender : MonoBehaviour
{
    public string phoneID = "1";
    [SerializeField] private TextMeshProUGUI text;
    public string serverIP = "192.168.0.0";
    public int port = 12345;
    
    private UdpClient _udpClient;
    [NonSerialized] public float stopTextTime = 0f;
    
    private void Start()
    {
        _udpClient = new UdpClient();
        if (AttitudeSensor.current != null) InputSystem.EnableDevice(AttitudeSensor.current); // Enable attitudeSensor
        if (Accelerometer.current != null) InputSystem.EnableDevice(Accelerometer.current); // Enable accelerometer
    }

    private void Update()
    {
        if (AttitudeSensor.current != null && Gyroscope.current != null && Accelerometer.current != null)
        {
            // Write message 
            string message = string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0},{1},{2},{3}",
                phoneID,
                AttitudeSensor.current.attitude.value.x,
                Gyroscope.current.angularVelocity.value.y,
                Accelerometer.current.acceleration.value.y);
            
            // Send message off into the great abyss!
            byte[] data = Encoding.UTF8.GetBytes(message);
            _udpClient.Send(data, data.Length, serverIP, port);
        }
        
        // Debug
        stopTextTime -= Time.deltaTime;
        if (stopTextTime <= 0f) SetText();
    }

    private void SetText()
    {
        AttitudeSensor att = AttitudeSensor.current;
        string attStatus = att?.enabled == true ? "<color=green>Enabled</color>" : "<color=red>Disabled</color>";
        Accelerometer acc = Accelerometer.current;
        string accStatus = acc?.enabled == true ? "<color=green>Enabled</color>" : "<color=red>Disabled</color>";
        Gyroscope gyr = Gyroscope.current;
        string gyrStatus = gyr?.enabled == true ? "<color=green>Enabled</color>" : "<color=red>Disabled</color>";
        List<float?> messageList = new List<float?>{att?.attitude.value.x, gyr?.angularVelocity.value.y, acc?.acceleration.value.y};
        string messageString = string.Join(",", messageList);

        text.text = $"Att {attStatus}; Acc: {accStatus}; Gyr: {gyrStatus}\nMessage: {messageString}\nPort: {port}";
    }
    
    private void OnApplicationQuit()
    {
        // Quit cleanly
        _udpClient.Close();
    }
}