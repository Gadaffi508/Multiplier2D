using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public string playerName;
    internal bool _islocalPlayer;
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
                if (_islocalPlayer is true)
                    _manager.localPlayerScore++;
                else
                    _manager.globalPlayerScore++;
                
                _manager.UpdateScore();
            }
        }
    }
}
