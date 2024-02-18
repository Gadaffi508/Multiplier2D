using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;
using UnityEngine.UI;

public class SteamLobby : MonoBehaviour
{
    public static SteamLobby Instance;
    
    protected Callback<LobbyCreated_t> LobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> JoinRequest;
    protected Callback<LobbyEnter_t> LobbyEntered;
    
    protected Callback<LobbyMatchList_t> LobbyList;
    protected Callback<LobbyDataUpdate_t> LobbyDataUpdated;

    public List<CSteamID> lobbyIDs = new List<CSteamID>();
    
    public ulong CurrentLobbyID;
    private const string HostAddressKey = "HostAddress";
    private CustomNetworkManager cmanager;
    private void Start()
    {
        if(!SteamManager.Initialized) return;

        if (Instance == null) Instance = this;
        
        cmanager = GetComponent<CustomNetworkManager>();
        LobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        JoinRequest = Callback<GameLobbyJoinRequested_t>.Create(OnJoinRequest);
        LobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
        
        LobbyList = Callback<LobbyMatchList_t>.Create(OnGetLobbyList);
        LobbyDataUpdated = Callback<LobbyDataUpdate_t>.Create(OnGetLobbyData);
    }
    private void OnLobbyCreated(LobbyCreated_t callback)
    {
        if(callback.m_eResult != EResult.k_EResultOK) return;
        
        Debug.Log("Lobby Created Succesfully");
        cmanager.StartHost();
        
        SteamMatchmaking.SetLobbyData(
            new CSteamID(callback.m_ulSteamIDLobby), HostAddressKey, SteamUser.GetSteamID().ToString());
        
        SteamMatchmaking.SetLobbyData(
            new CSteamID(callback.m_ulSteamIDLobby), "name", SteamFriends.GetPersonaName().ToString() + " 's LOBBY");
    }
    
    private void OnJoinRequest(GameLobbyJoinRequested_t callback)
    {
        Debug.Log("Request To Join Lobyy");
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }

    private void OnLobbyEntered(LobbyEnter_t callback)
    {
        CurrentLobbyID = callback.m_ulSteamIDLobby;
        if(NetworkServer.active) return;
        
        cmanager.networkAddress = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby ), HostAddressKey);
        cmanager.StartClient();
    }

    public void JoinLobby(CSteamID lobbyID)
    {
        SteamMatchmaking.JoinLobby(lobbyID);
    }

    public void GetLobbiesList()
    {
        if(lobbyIDs.Count > 0) lobbyIDs.Clear();
        SteamMatchmaking.RequestLobbyList();
        SteamMatchmaking.AddRequestLobbyListFilterSlotsAvailable(1);
    }
    
    private void OnGetLobbyList(LobbyMatchList_t result)
    {
        if(LobbiesListManager.Instance.listOfLobbies.Count > 0) LobbiesListManager.Instance.DestroyLobbies();
        for (int i = 0; i < result.m_nLobbiesMatching; i++)
        {
            CSteamID lobbyId = SteamMatchmaking.GetLobbyByIndex(i);
            lobbyIDs.Add(lobbyId);
            SteamMatchmaking.RequestLobbyData(lobbyId);
        }
    }
    
    private void OnGetLobbyData(LobbyDataUpdate_t result)
    {
        LobbiesListManager.Instance.DisaplayLobbies(lobbyIDs,result);
    }
    
    public void Host()
    {
        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePublic, 2);
        SteamLobbyController.Instance.UpdatePlayerList();
    }

    public void LeaveLobby(string sceneName)
    {
        SteamMatchmaking.LeaveLobby(new CSteamID(CurrentLobbyID));
        SteamMatchmaking.DeleteLobbyData(new CSteamID(CurrentLobbyID),"name");
        SteamLobbyController.Instance.ClearPlayerList();
        CurrentLobbyID = 0;
        cmanager.StartGame(sceneName);
        Debug.Log("Disconnected");
    }
}



























