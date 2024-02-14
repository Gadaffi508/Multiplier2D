using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class PlayerMove : NetworkBehaviour
{
    public float speed = 1;
    public string gameSceneName;
    public GameObject playerSprite;
    public Text playerName;
    
    private SteamPlayerController _steamPlayerController;
    private void Start()
    {
        playerSprite.SetActive(false);
        _steamPlayerController = GetComponent<SteamPlayerController>();
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name == gameSceneName)
        {
            playerSprite.SetActive(true);
             
            playerName.text = _steamPlayerController.PlayerName;
        }

        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        CmdMove(x,y);
    }
    
    [Command]
    private void CmdMove(float x, float y)
    {
        RpcMove(x,y);
    }
    
    [ClientRpc]
    private void RpcMove(float x, float y)
    {
        Vector3 direction = new Vector3(x,y);

        transform.Translate(direction * speed * Time.deltaTime);
    }
}
