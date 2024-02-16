using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public string playerName;
    private GameSceneManager _manager;

    private void Start()
    {
        _manager = GameObject.FindGameObjectWithTag("Manager").gameObject.GetComponent<GameSceneManager>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.TryGetComponent(out SteamPlayerController playerController))
        {
            if (playerController.PlayerName != playerName)
            {
                _manager.globalPlayerScore++;
                _manager.localPlayerScore++;
                _manager.UpdateScore();
            }
        }
    }
}
