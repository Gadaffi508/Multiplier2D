using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using Steamworks;
using UnityEngine.SceneManagement;

public class CustomNetworkManager : NetworkManager
{
    [SerializeField] private SteamPlayerController gamePlayerPrefabs;
    public List<SteamPlayerController> GamePlayer { get; } = new List<SteamPlayerController>();
    
    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        if (SceneManager.GetActiveScene().name == "Lobby")
        {
            SteamPlayerController GamePlayerInstance = Instantiate(gamePlayerPrefabs);
            GamePlayerInstance.ConnectionId = conn.connectionId;
            GamePlayerInstance.PlayerId = GamePlayer.Count + 1;
            GamePlayerInstance.PlayerSteamId = (ulong)SteamMatchmaking.GetLobbyMemberByIndex(
                (CSteamID)SteamLobby.Instance.CurrentLobbyID,
                GamePlayer.Count);
            NetworkServer.AddPlayerForConnection(conn,GamePlayerInstance.gameObject);
        }
    }
    
    public void StartGame(string SceneName)
    {
        ServerChangeScene(SceneName);
    }
}


