using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

// this script control that the player removed from the game


public class PlayerSpawner : MonoBehaviour
{
    public static PlayerSpawner instance;
    public GameObject playerPrefab;
    private GameObject player;
    // Start is called before the first frame update

    public void Awake()
    {
        instance = this;
    }

    void Start()
    {
        if (PhotonNetwork.IsConnected)
        {
            SpawnPlayer();
        }
    }

    public void SpawnPlayer()
    {
        Transform spawnPoint = SpawnManager.instance.GetSpawnPoint();

        player =PhotonNetwork.Instantiate(playerPrefab.name ,spawnPoint.position ,spawnPoint.rotation);
    }
}
