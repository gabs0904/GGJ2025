using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Alteruna;

public class GameController : MonoBehaviour {
    [SerializeField] private Multiplayer multiplayer;
    private int timer = 180;
    private float counter = 0;
    private bool gameInProgress = false;


    public void StartGame() {/*
        // Check if there are exactly 2 players in the lobby
        List<Alteruna.Avatar> players = multiplayer.GetAvatars();

        if (players.Count == 2) {
            gameInProgress = true;
            timer = 180; // Reset the timer
            counter = 0;

            // Synchronize game start across all players
            multiplayer.InvokeRemoteProcedure("SyncGameStart", 65535, "start");
        } else {
            Debug.LogWarning("Not enough players to start the game!");
        }*/

        // teleport players na mapa i enable movement
    }

    public void PlayerDied(int winnerId) {
        /*
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
        */
    }
}
