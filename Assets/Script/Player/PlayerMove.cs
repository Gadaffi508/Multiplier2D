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
    
    public float rotationSpeed = 5f;
    public Transform weaponPivot;
    public Transform bulletPos;
    public GameObject bullet;
    
    private SteamPlayerController _steamPlayerController;
    private Rigidbody2D _rb;
    private float x, y;
    private void Start()
    {
        playerSprite.SetActive(false);
        _steamPlayerController = GetComponent<SteamPlayerController>();
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name == gameSceneName)
        {
            playerSprite.SetActive(true);
             
            playerName.text = _steamPlayerController.PlayerName;

            CmdWeaponRotate();

            if (Input.GetMouseButtonDown(0)) CmdShoot();
        }
        else
            playerSprite.SetActive(false);

        x = Input.GetAxis("Horizontal");
        y = Input.GetAxis("Vertical");
    }

    private void FixedUpdate()
    {
        CmdMove(x,y);
    }

    #region Commands

    [Command]
    private void CmdMove(float x, float y) => RpcMove(x,y);
    [Command]
    private void CmdShoot() => Shoot();
    [Command]
    private void CmdWeaponRotate() => WeaponRotate();

    #endregion

    #region Move

    [ClientRpc]
    private void RpcMove(float x, float y)
    {
        Vector3 direction = new Vector3(x,y);
        _rb.velocity = direction * speed * Time.deltaTime;
    }

    #endregion

    #region WeaponRotateFunc
    [ClientRpc]
    private void WeaponRotate()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0f;

        Vector3 direction = mousePosition - weaponPivot.position;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion desiredRotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
        weaponPivot.rotation = Quaternion.Lerp(weaponPivot.rotation, desiredRotation, rotationSpeed * Time.deltaTime);
    }
    #endregion

    #region ShootFunc
    [ClientRpc]
    private void Shoot()
    {
        Quaternion bulletRotation = bulletPos.rotation;

        GameObject _bullet = Instantiate(bullet, bulletPos.position, bulletRotation);

        Vector2 bulletDirection = bulletRotation * Vector2.right;
        _bullet.GetComponent<Rigidbody2D>().AddForce(bulletDirection * 1000);
        _bullet.GetComponent<Bullet>().playerName = _steamPlayerController.PlayerName;
        _bullet.GetComponent<Bullet>()._islocalPlayer = _steamPlayerController.isLocalPlayer;

        Destroy(_bullet, 5f);
    }
    #endregion
}
