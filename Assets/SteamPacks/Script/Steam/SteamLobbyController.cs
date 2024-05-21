using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;
using UnityEngine.UI;
using System.Linq;

public class SteamLobbyController : MonoBehaviour
{
    public static SteamLobbyController Instance;
    
    public Text LobbyNameText;
    
    public GameObject PlayerListViewContent;
    public GameObject PlayerListItemPrefab;
    public GameObject LocalPlayerObject;
    
    public ulong CurrentLobbyID;
    public bool PlayerItemCreated = false;
    private List<SteamPlayerItem> _playerItems = new List<SteamPlayerItem>();
    public SteamPlayerController LocalPlayerController;

    private SteamLobby _steamLobby;
    private CustomNetworkManager _manager;

    private CustomNetworkManager Manager
    {
        get
        {
            if (_manager != null)
            {
                return _manager;
            }
            
            return _manager = CustomNetworkManager.singleton as CustomNetworkManager;
        }
    }
    
    public Button StartGameButton;
    public Text ReadyButtonText;

    public Text LobbyId;
    
    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public void ReadyPlayer()
    {
        LocalPlayerController.ChangeReady();
        ReadyButtonText.text = LocalPlayerController.Ready ? "Unready" : "Ready";
    }
    public void CheckIfAllReady()
    {
        bool allReady = Manager.GamePlayer.All(player => player.Ready);
        if (allReady)
        {
            // All players are ready, perform necessary actions
        }
    }
    public void UpdateLobbyName()
    {
        CurrentLobbyID = Manager.GetComponent<SteamLobby>().CurrentLobbyID;
        LobbyNameText.text = SteamMatchmaking.GetLobbyData(new CSteamID(CurrentLobbyID), "name");
        
        LobbyId.text = "" +CurrentLobbyID;
    }

    public void UpdatePlayerList()
    {
        foreach (SteamPlayerController player in Manager.GamePlayer)
        {
            if (_playerItems.Any(item => item.ConnectionId == player.ConnectionId))
                continue;

            GameObject newPlayerItem = Instantiate(PlayerListItemPrefab, PlayerListViewContent.transform);
            SteamPlayerItem newPlayerItemScript = newPlayerItem.GetComponent<SteamPlayerItem>();

            newPlayerItemScript.PlayerName = player.PlayerName;
            newPlayerItemScript.ConnectionId = player.ConnectionId;
            newPlayerItemScript.PlayerSteamID = player.PlayerSteamId;
            newPlayerItemScript.Ready = player.Ready;
            newPlayerItemScript.SetPlayerValues();
            
            _playerItems.Add(newPlayerItemScript);
        }

        List<SteamPlayerItem> playerItemsToRemove = _playerItems.Where(item => !Manager.GamePlayer.Any(player => player.ConnectionId == item.ConnectionId)).ToList();
        foreach (SteamPlayerItem itemToRemove in playerItemsToRemove)
        {
            _playerItems.Remove(itemToRemove);
        }
        
        CheckIfAllReady();
    }
    public void FindLocalPlayer()
    {
        LocalPlayerObject = GameObject.Find("LocalGamePlayer");
        LocalPlayerController = LocalPlayerObject.GetComponent<SteamPlayerController>();
    }

    public void StartGame(string sceneName)
    {
        LocalPlayerController.CanStartGame(sceneName);
    }
    public void ClearPlayerList()
    {
        _playerItems.Clear();
    }

    public void LobyCreate()
    {
        SteamLobby.Instance.Host();
    }
}















