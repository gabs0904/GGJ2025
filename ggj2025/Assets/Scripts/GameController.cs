/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Alteruna;

public class GameController : MonoBehaviour {
    [SerializeField] private Multiplayer multiplayer;
    private int timer = 180;
    private float counter = 0;
    private bool gameInProgress = false;

    private void Update() {
        // Handle the timer if the game is in progress
        if (gameInProgress) {
            counter += Time.deltaTime;

            if (counter >= 1) {
                counter--;
                timer--;

                // Synchronize timer to all players as a single string
                MonoBehaviour action = SyncTimer(UserId.All, timer.ToString());
                multiplayer.Invoke("SyncTimer", UserId.All, timer.ToString());

                if (timer <= 0) {
                    TimeOut();
                }
            }
        }
    }

    public void StartGame() {
        // Check if there are exactly 2 players in the lobby
        List<Player> players = multiplayer.Players;
        if (players.Count == 2) {
            gameInProgress = true;
            timer = 180; // Reset the timer
            counter = 0;

            // Synchronize game start across all players
            multiplayer.Invoke("SyncGameStart", UserId.All, "start");
        } else {
            Debug.LogWarning("Not enough players to start the game!");
        }
    }

    public void PlayerDied(int winnerId) {
        // Get the winner's name
        Player winner = multiplayer.GetPlayer(winnerId);
        string winnerName = winner != null ? winner.Name : "Unknown Player";

        // Combine winner ID and name into a single string
        string winnerInfo = $"{winnerId}|{winnerName}";

        // Synchronize the winner announcement with all players
        multiplayer.Invoke("SyncGameOver", UserId.All, winnerInfo);

        // Stop the game
        gameInProgress = false;
    }

    private void SyncTimer(string syncedTimerString) {
        // Parse the timer from the string
        if (int.TryParse(syncedTimerString, out int syncedTimer)) {
            timer = syncedTimer;
        }
    }

    private void SyncGameStart(string command) {
        if (command == "start") {
            // Trigger any actions when the game starts
            Debug.Log("Game has started!");
        }
    }

    private void SyncGameOver(string winnerInfo) {
        // Parse winner ID and name from the string
        string[] parts = winnerInfo.Split('|');
        if (parts.Length == 2) {
            int winnerId = int.Parse(parts[0]);
            string winnerName = parts[1];

            // Trigger any actions when a player wins
            Debug.Log($"Game Over! Winner: {winnerName} (ID: {winnerId})");
        } else {
            Debug.LogWarning("Invalid winner data received.");
        }
    }
}
*/