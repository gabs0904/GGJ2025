using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class NetworkUtils : MonoBehaviour {
    public static string GetLocalIPAddress() {
        string localIP = "Not Available";

        try {
            using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0)) {
                socket.Connect("8.8.8.8", 65530);  // Connect to an external server (Google DNS)
                IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                localIP = endPoint.Address.ToString();
            }
        } catch (System.Exception) {
            Debug.LogError("Error getting local IP address");
        }

        return localIP;
    }
}
