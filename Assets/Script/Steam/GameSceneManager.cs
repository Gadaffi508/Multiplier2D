using System;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class GameSceneManager : NetworkBehaviour
{
    public int localPlayerScore;
    public int globalPlayerScore;
    public string sceneName;
    public Text localPlayerScoreText;
    public Text globalPlayerScoreText;
    
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
        UpdateScore();
    }

    [Command]
    public void UpdateScore()
    {
        localPlayerScoreText.text = "Local Player Score " + localPlayerScore;
        globalPlayerScoreText.text = "Global Player Score " + globalPlayerScore;
    }

    public void LeaveGame()
    {
        SteamLobby.Instance.LeaveLobby(sceneName);
    }
}