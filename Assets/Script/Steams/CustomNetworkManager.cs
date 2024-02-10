using System.Collections.Generic;
using Mirror;
using Steamworks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CustomNetworkManager : NetworkManager
{
    [SerializeField] private SPlayerObjectController gamePlayerPrefabs;
    public List<SPlayerObjectController> GamePlayer { get; } = new List<SPlayerObjectController>();

    
    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        
            SPlayerObjectController GamePlayerInstance = Instantiate(gamePlayerPrefabs);
            GamePlayerInstance.ConnectionId = conn.connectionId;
            GamePlayerInstance.PlayerId = GamePlayer.Count + 1;
            GamePlayerInstance.PlayerSteamId = (ulong)SteamMatchmaking.GetLobbyMemberByIndex(
                (CSteamID)SteamLobby.Instance.CurrentLobbyID,
                GamePlayer.Count
            );
            NetworkServer.AddPlayerForConnection(conn,GamePlayerInstance.gameObject);
        
    }
    
    public void StartGame(string SceneName)
    {
        ServerChangeScene(SceneName);
        if (SteamMatchmaking.GetNumLobbyMembers((CSteamID)SteamLobby.Instance.CurrentLobbyID) != 2)
        {
            Debug.Log("You are the only person in the match..");
        }
    }
}
