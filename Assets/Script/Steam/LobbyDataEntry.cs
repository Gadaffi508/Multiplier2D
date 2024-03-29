using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Steamworks;

public class LobbyDataEntry : MonoBehaviour
{
    public CSteamID lobbyId;
    public string lobbyName;
    public Text lobbyNameText;
    public Text MemebersText;

    public void SetLobbyData()
    {
        if(lobbyName == "") lobbyNameText.text = "Empty";
        else lobbyNameText.text = lobbyName;
    }

    public void JoinLobby()
    {
        SteamLobby.Instance.JoinLobby(lobbyId);
    }
}
