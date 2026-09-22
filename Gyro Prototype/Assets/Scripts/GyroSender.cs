// This is a little script that sends gyroscope data through the UDP-client (which uses a IP network)
// The data it sends is formatted as: "ID,x,y,z", where ID is an assignable device ID, and xyz is the rotation of the gyroscope

using System;
using UnityEngine;
using System.Net.Sockets;
using System.Text;
using TMPro;
using UnityEngine.InputSystem;
using Gyroscope = UnityEngine.InputSystem.Gyroscope;

public class GyroSender : MonoBehaviour
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
        if (Gyroscope.current != null) InputSystem.EnableDevice(Gyroscope.current); // Enable that device
    }

    private void Update()
    {
        if (Gyroscope.current != null)
        {
            // Write message 
            string message = string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0},{1},{2},{3}", 
                phoneID,
                Gyroscope.current.angularVelocity.x.ReadValue(),
                Gyroscope.current.angularVelocity.y.ReadValue(),
                Gyroscope.current.angularVelocity.z.ReadValue());
            
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
        var gyro = Gyroscope.current;
        string status = gyro?.enabled == true ? "<color=green>Enabled</color>" : "<color=red>Disabled</color>";
        Vector3 rate = gyro?.angularVelocity.ReadValue() ?? Vector3.zero;

        text.text = $"Gyro {status}\nRate: {rate}";
    }
    
    private void OnApplicationQuit()
    {
        // Quit cleanly
        _udpClient.Close();
    }
}