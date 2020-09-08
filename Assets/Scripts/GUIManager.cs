using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GUIManager : MonoBehaviour
{
    public PlayerGUI[] playerGUI;

    public void UpdatePlayerGUI()
    {
        for (int i = 0; i < GameSettings.playersPlaying; i++)
        {
            playerGUI[i].UpdateEverything(GameManager.Instance.players[i]);
        }
    }

    public void SetDeath(int index, Action OnRespawn)
    {
        playerGUI[index].StartRespawnTimer(OnRespawn);
    }

    public void TakeDamage(int index)
    {
        playerGUI[index].FlashRed();
    }

}
