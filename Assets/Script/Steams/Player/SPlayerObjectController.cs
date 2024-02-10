using Steamworks;
using Mirror;
using UnityEngine;

public class SPlayerObjectController : NetworkBehaviour
{
    [SyncVar] 
    public int ConnectionId;
    [SyncVar] 
    public int PlayerId;
    [SyncVar] 
    public ulong PlayerSteamId;
    
    [SyncVar(hook = nameof(PlayerNameUpdate))]
    public string PlayerName;
    
    [SyncVar(hook = nameof(PlayerReadyUpdate))]
    public bool Ready;
    
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

    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    public void PlayerReadyUpdate(bool oldValue, bool newValue)
    {
        if (isServer)
        {
            this.Ready = newValue;
        }
        
        if (isClient)
        {
            LobbyController.Instance.UpdatePlayerList();
        }
    }

    [Command]
    private void CMdSetPlayerReady()
    {
        this.PlayerReadyUpdate(this.Ready,!this.Ready);
    }

    public void ChangeReady()
    {
        if (authority) CMdSetPlayerReady();
    }

    public override void OnStartAuthority()
    {
        CmdSetPlayerName(SteamFriends.GetPersonaName().ToString());
        gameObject.name = "LocalGamePlayer";
        LobbyController.Instance.FindLocalPlayer();
        LobbyController.Instance.UpdateLobbyName();
    }

    public override void OnStartClient()
    {
        Manager.GamePlayer.Add(this);
        LobbyController.Instance.UpdateLobbyName();
        LobbyController.Instance.UpdatePlayerList();
    }

    public override void OnStopClient()
    {
        Manager.GamePlayer.Remove(this);
        LobbyController.Instance.UpdatePlayerList();
    }

    [Command]
    private void CmdSetPlayerName(string PlayerName)
    {
        this.PlayerNameUpdate(this.PlayerName,PlayerName);
    }

    public void PlayerNameUpdate(string oldValue, string newValue)
    {
        if (isServer)
        {
            this.PlayerName = newValue;
        }
        
        if (isClient)
        {
            LobbyController.Instance.UpdatePlayerList();
        }
    }
    
    public void CanStartGame(string SceneName)
    {
        if (authority)
        {
            CmdStartGame(SceneName);
        }
    }
    
    [Command]
    public void CmdStartGame(string SceneName)
    {
        manager.StartGame(SceneName);
    }
}
