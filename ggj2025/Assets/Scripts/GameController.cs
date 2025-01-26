using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Alteruna;

public class GameController : MonoBehaviour
{
    Multiplayer multiplayer;
    int timer = 180;
    float counter = 0;

    private void Update() {
        counter += Time.deltaTime;

        if (counter >= 1) {
            counter--;
            timer--;
        }
    }

    public void StartGame() {
        // called when a button is pressed if there are 2 players in the lobby

    }

    public void PlayerDied() {

    }

    public void TimeOut() {

    }
}
