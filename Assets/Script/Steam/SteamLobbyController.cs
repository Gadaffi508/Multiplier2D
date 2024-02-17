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
    
    private CustomNetworkManager manager;

    private CustomNetworkManager Manager
    {
        get
        {
            if (manager != null)
            {
                return manager;
            }
            
            return manager = CustomNetworkManager.singleton as CustomNetworkManager;
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
    }

    public void UpdateButton()
    {
        if (LocalPlayerController.Ready) ReadyButtonText.text = "Unready";
        else ReadyButtonText.text = "Ready";
        
        LobbyId.text = "" +CurrentLobbyID;
    }

    public void CheckIfAllReady()
    {
        bool AllReady = false;

        foreach (SteamPlayerController player in Manager.GamePlayer)
        {
            if (player.Ready) AllReady = true;
            else
            {
                AllReady = false;
                break;
            }
        }
    }
    
    public void UpdateLobbyName()
    {
        CurrentLobbyID = Manager.GetComponent<SteamLobby>().CurrentLobbyID;
        LobbyNameText.text = SteamMatchmaking.GetLobbyData(new CSteamID(CurrentLobbyID), "name");
    }

    public void UpdatePlayerList()
    {
        if (!PlayerItemCreated) CreateHostPlayerItem();
        if (_playerItems.Count < Manager.GamePlayer.Count) CreateClientPlayerItem();
        if (_playerItems.Count > Manager.GamePlayer.Count) RemovePlayerItem();
        if (_playerItems.Count == Manager.GamePlayer.Count) UpdatePlayerItem();
    }
    public void FindLocalPlayer()
    {
        LocalPlayerObject = GameObject.Find("LocalGamePlayer");
        LocalPlayerController = LocalPlayerObject.GetComponent<SteamPlayerController>();
    }

    public void CreateHostPlayerItem()
    {
        foreach (SteamPlayerController player in Manager.GamePlayer)
        {
            GameObject NewPlayerItem = Instantiate(PlayerListItemPrefab) as GameObject;
            SteamPlayerItem NewPlayerItemScript = NewPlayerItem.GetComponent<SteamPlayerItem>();

            NewPlayerItemScript.PlayerName = player.PlayerName;
            NewPlayerItemScript.ConnectionId = player.ConnectionId;
            NewPlayerItemScript.PlayerSteamID = player.PlayerSteamId;
            NewPlayerItemScript.Ready = player.Ready;
            NewPlayerItemScript.SetPlayerValues();
            NewPlayerItem.transform.SetParent(PlayerListViewContent.transform);
            NewPlayerItem.transform.localScale = Vector3.one;
            
            _playerItems.Add(NewPlayerItemScript);
        }

        PlayerItemCreated = true;
    }
    
    public void CreateClientPlayerItem()
    {
        foreach (SteamPlayerController player in Manager.GamePlayer)
        {
            if (!_playerItems.Any(b => b.ConnectionId == player.ConnectionId))
            {
                GameObject NewPlayerItem = Instantiate(PlayerListItemPrefab) as GameObject;
                SteamPlayerItem NewPlayerItemScript = NewPlayerItem.GetComponent<SteamPlayerItem>();

                NewPlayerItemScript.PlayerName = player.PlayerName;
                NewPlayerItemScript.ConnectionId = player.ConnectionId;
                NewPlayerItemScript.PlayerSteamID = player.PlayerSteamId;
                NewPlayerItemScript.Ready = player.Ready;
                NewPlayerItemScript.SetPlayerValues();
            
                NewPlayerItem.transform.SetParent(PlayerListViewContent.transform);
                NewPlayerItem.transform.localScale = Vector3.one;
            
                _playerItems.Add(NewPlayerItemScript);
            }
        }
    }
    
    public void UpdatePlayerItem()
    {
        foreach (SteamPlayerController player in Manager.GamePlayer)
        {
            foreach (SteamPlayerItem PlayerListItemScript in _playerItems)
            {
                if (PlayerListItemScript.ConnectionId == player.ConnectionId)
                {
                    PlayerListItemScript.PlayerName = player.PlayerName;
                    PlayerListItemScript.Ready = player.Ready;
                    PlayerListItemScript.SetPlayerValues();
                    if (player == LocalPlayerController)
                    {
                        UpdateButton();
                    }
                }
            }
        }
        CheckIfAllReady();
    }
    
    public void RemovePlayerItem()
    {
        List<SteamPlayerItem> playerListItemToRemove = new List<SteamPlayerItem>();

        foreach (SteamPlayerItem playerListItem in _playerItems)
        {
            if (!Manager.GamePlayer.Any(b => b.ConnectionId == playerListItem.ConnectionId))
            {
                playerListItemToRemove.Add(playerListItem);
            }
        }

        if (playerListItemToRemove.Count > 0)
        {
            foreach (SteamPlayerItem playerlistItemToRemove in playerListItemToRemove)
            {
                GameObject ObjectToRemove = playerlistItemToRemove.gameObject;
                _playerItems.Remove(playerlistItemToRemove);
                Destroy(ObjectToRemove);
                ObjectToRemove = null;
            }
        }
    }

    public void StartGame(string sceneName)
    {
        LocalPlayerController.CanStartGame(sceneName);
    }

    public void LobbyCreate()
    {
        SteamLobby.Instance.HostLobby();
    }
}















