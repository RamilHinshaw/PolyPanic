using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    public void SetPlayer(int playersPlaying)
    {
        GameSettings.playersPlaying = playersPlaying;
    }

    public void SetPlayerCharacter(int playerID, int characterID)
    {
        GameSettings.playersCharacter[playerID] = characterID;
    }
}
