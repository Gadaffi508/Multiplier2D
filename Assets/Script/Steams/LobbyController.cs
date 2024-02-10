using Steamworks;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LobbyController : MonoBehaviour
{
    public static LobbyController Instance;
    
    public Text LobbyNameText;
    
    public GameObject PlayerListViewContent;
    public GameObject PlayerListItemPrefab;
    public GameObject LocalPlayerObject;
    
    public ulong CurrentLobbyID;
    public bool PlayerItemCreated = false;
    private List<SPlayerLıstItem> _playerItems = new List<SPlayerLıstItem>();
    public SPlayerObjectController LocalPlayerController;
    
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

        foreach (SPlayerObjectController player in Manager.GamePlayer)
        {
            if (player.Ready) AllReady = true;
            else
            {
                AllReady = false;
                break;
            }
        }

        if (AllReady)
        {
            if (LocalPlayerController.PlayerId == 1)
            {
                StartGameButton.interactable = true;
            }
            else
            {
                StartGameButton.interactable = false;
            }
        }
        else
        {
            StartGameButton.interactable = false;
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
    
    //Yerel oyuncu bulma
    public void FindLocalPlayer()
    {
        LocalPlayerObject = GameObject.Find("LocalGamePlayer");
        LocalPlayerController = LocalPlayerObject.GetComponent<SPlayerObjectController>();
    }

    public void CreateHostPlayerItem()
    {
        foreach (SPlayerObjectController player in Manager.GamePlayer)
        {
            GameObject NewPlayerItem = Instantiate(PlayerListItemPrefab) as GameObject;
            SPlayerLıstItem NewPlayerItemScript = NewPlayerItem.GetComponent<SPlayerLıstItem>();

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
        foreach (SPlayerObjectController player in Manager.GamePlayer)
        {
            if (!_playerItems.Any(b => b.ConnectionId == player.ConnectionId))
            {
                GameObject NewPlayerItem = Instantiate(PlayerListItemPrefab) as GameObject;
                SPlayerLıstItem NewPlayerItemScript = NewPlayerItem.GetComponent<SPlayerLıstItem>();

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
        foreach (SPlayerObjectController player in Manager.GamePlayer)
        {
            foreach (SPlayerLıstItem PlayerListItemScript in _playerItems)
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
        List<SPlayerLıstItem> playerListItemToRemove = new List<SPlayerLıstItem>();

        foreach (SPlayerLıstItem playerListItem in _playerItems)
        {
            if (!Manager.GamePlayer.Any(b => b.ConnectionId == playerListItem.ConnectionId))
            {
                playerListItemToRemove.Add(playerListItem);
            }
        }

        if (playerListItemToRemove.Count > 0)
        {
            foreach (SPlayerLıstItem playerlistItemToRemove in playerListItemToRemove)
            {
                GameObject ObjectToRemove = playerlistItemToRemove.gameObject;
                _playerItems.Remove(playerlistItemToRemove);
                Destroy(ObjectToRemove);
                ObjectToRemove = null;
            }
        }
    }

    public void StartGame(string SceneName)
    {
        if (SteamMatchmaking.GetNumLobbyMembers((CSteamID)SteamLobby.Instance.CurrentLobbyID) == 2)
        {
            Debug.Log("Connection.");
        }
        else
        {
            Debug.Log("You need 1 person.");
        }
        LocalPlayerController.CanStartGame(SceneName);
    }
}
