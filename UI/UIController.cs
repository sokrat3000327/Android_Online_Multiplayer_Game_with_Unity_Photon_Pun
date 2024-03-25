using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class UIController : MonoBehaviour
{

  
    public static UIController instance;

    private void Awake()
    {
        instance = this;

    }

    public GameObject endScreen;
    public TextMeshProUGUI scoreText;
    // the leaderBoard UI
    public GameObject leaderboard;

    //// the script related to the leaderBoard
    public LeaderBoard leaderboardPlayerDisplay;


    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

}
