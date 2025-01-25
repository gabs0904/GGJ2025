using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour {
    public static GameManager Instance;

    public NetworkVariable<float> Timer = new NetworkVariable<float>(60f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public GameObject playerPrefab;
    public Transform[] spawnPoints;

    private void Awake() {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Update() {
        if (IsServer) {
            Timer.Value -= Time.deltaTime;
            if (Timer.Value <= 0f) {
                Timer.Value = 0f;
                EndGame();
            }
        }
    }

    private void EndGame() {
        // End game logic here
        Debug.Log("Game Over!");
    }

    // Spawns the player at the spawn point
    private void SpawnPlayer(ulong clientId) {
        if (spawnPoints.Length > 0) {
            int spawnIndex = (int)((int)clientId % spawnPoints.Length); // Ensure each player gets a unique spawn
            Transform spawnPoint = spawnPoints[spawnIndex];

            GameObject player = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
            player.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void StartGameServerRpc() {
        ChangeScene("game");
        Timer.Value = 60f; // Reset timer and start the game
        SpawnPlayers();
    }

    // Spawn all players when the game starts
    private void SpawnPlayers() {
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds) {
            SpawnPlayer(clientId);
        }
    }

    // Call this method to switch scenes for all players
    public void ChangeScene(string sceneName) {
        print("about to change scene");
        if (IsServer) {
            print("is server");
            // This will switch the scene for all clients connected to the server
            NetworkManager.Singleton.SceneManager.LoadScene(sceneName, UnityEngine.SceneManagement.LoadSceneMode.Single);
        }
    }
}
