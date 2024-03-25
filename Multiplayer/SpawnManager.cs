using System.Collections;
using System.Collections.Generic;
using UnityEngine;



// i must complete after that i should reposition the player in different positions when the game start
public class SpawnManager : MonoBehaviour
{
   
    public static SpawnManager instance;
    public Transform[] spawnPoints;

    private void Awake()
    {
        instance = this;
    }
    
    
    void Start()
    {
        //to deactivate all of these objects 
        foreach (Transform spam in spawnPoints)
        {
            spam.gameObject.SetActive(false);      
        }
    }

  
    void Update()
    {
        
    }
    public Transform GetSpawnPoint()
    {
        return spawnPoints[Random.Range(0,spawnPoints.Length)];
    }
}
