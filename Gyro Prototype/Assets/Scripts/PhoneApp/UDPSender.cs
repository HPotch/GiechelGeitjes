// This is a little script that sends attitudeSensor data through the UDP-client (which uses a IP network)
// The data it sends is formatted as: "ID,x,y", where ID is an assignable device ID, x is the euler x rotation of the attitudeSensor and y is the accelerometer's y

using System;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Text;
using TMPro;
using UnityEngine.InputSystem;


public class UDPSender : MonoBehaviour
{
    public string phoneID = "1";
    [SerializeField] private List<TextMeshProUGUI> debug;
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
        if (AttitudeSensor.current != null && Accelerometer.current != null)
        {
            // Write message 
            string message = string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0},{1},{2},{3}",
                phoneID,
                AttitudeSensor.current.attitude.value.x,
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
        List<float?> messageList = new List<float?>{att?.attitude.value.x, acc?.acceleration.value.y};
        string messageString = string.Join(",", messageList);

        foreach (TextMeshProUGUI text in debug)
            text.text = $"Att {attStatus}; Acc: {accStatus}\nMessage: {messageString}\nPort: {port}";
    }
    
    private void OnApplicationQuit()
    {
        // Quit cleanly
        _udpClient.Close();
    }
}