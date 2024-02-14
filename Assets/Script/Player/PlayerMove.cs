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
        
        if(authority) Movement();
    }

    public void SetPosition()
    {
        transform.position = new Vector3(Random.Range(0,3),Random.Range(0,3));
    }

    public void Movement()
    {
        float X = Input.GetAxis("Horizontal");
        float Z = Input.GetAxis("Vertical");

        Vector2 moveDirection = new Vector2(X,Z);
        transform.Translate(moveDirection * Time.deltaTime * speed);
    }
}
