using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LobbyMenu : MonoBehaviour {
    public Button hostButton;
    public Button joinButton;
    public TMP_InputField ipInputField;  // Input for entering the host IP (for joining)

    public TMP_Text hostAdress;

    private void Start() {
        hostButton.onClick.AddListener(HostLobby);
        joinButton.onClick.AddListener(JoinLobby);
    }

    // Host a lobby (becomes server + client)
    private void HostLobby() {
        print("Host");
        // Start as host (server + client)
        NetworkManager.Singleton.StartHost();
        Debug.Log("Hosting game...");

        hostAdress.text = NetworkUtils.GetLocalIPAddress();

        GameManager.Instance.StartGameServerRpc();
    }

    // Join a lobby (client)
    private void JoinLobby() {
        print("Join");

        string ip = ipInputField.text;  // Host IP to join

        // Set the network address to the IP of the host
        NetworkManager.Singleton.GetComponent<Unity.Netcode.Transports.UTP.UnityTransport>().SetConnectionData(ip, 7777);

        // Start as client
        NetworkManager.Singleton.StartClient();
        Debug.Log("Joining game...");

        GameManager.Instance.StartGameServerRpc();
    }
}
