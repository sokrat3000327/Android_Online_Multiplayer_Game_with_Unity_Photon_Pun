using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using Photon.Realtime;
using ExitGames.Client.Photon;



//// IOnEventCallback use to detect events and when it occurs decide what doyou want

//// in this script the score equal 1 so we want to get one fruit

public class GamePlayManager4 : MonoBehaviourPunCallbacks, IOnEventCallback
{
    public static GamePlayManager4 instance;

    public List<PlayerInfo> allPlayers = new List<PlayerInfo>();
    private GameManager2 GM2;





    public EventCode theEvent;

    // index of the player
    private int index;
    public int scoreToWin;
    public Transform mapCamPoint;
    //this is the state waiting the game to start and to switch to playing state
    public GameState state = GameState.waiting;
    public float waiitAfterEnding = 5f;



    private void Awake()
    {
        instance = this;

    }


    //    // byte is a type of data that is smaller than int
    //    //it is useful when sending the data over the network
    //    // you can switch between them to select which event we choose
    //    // THIS IS the all states that will game depend on
    public enum EventCodes : byte
    {
        NewPlayer,
        ListPlayers,
        UpdateStat
    }

    //    // list of all data releated to leaderBoard UI
    private List<LeaderBoard> lboardplayers = new List<LeaderBoard>();


    // this related to end the game
    public enum GameState
    {
        waiting,
        playing,
        Ending
    }






    //    // Start is called before the first frame update
    void Start()
    {
        if (!PhotonNetwork.IsConnected)
        {
            SceneManager.LoadScene(0);
        }
        else
        {

            NewPlayerSend(PhotonNetwork.NickName);
            // we make sure that the game is okay
            state = GameState.playing;
            GM2 = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<GameManager2>();
            scoreToWin = GM2.myTargetScore1;

        }
    }

    //    // Update is called once per frame
    void Update()
    {


        //if i press the tab key show the leaderBoard else disappear the leaderBoard
        //if the game is ended do not show the leaderboard
        if (Input.GetKeyDown(KeyCode.Tab) && state != GameState.Ending)
        {
            if (UIController.instance.leaderboard.activeInHierarchy)
            {
                UIController.instance.leaderboard.SetActive(false);
            }
            else
            {
                showLeaderBoard();
            }
        }



    }


    public void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code < 200)
        {
            EventCodes theEvent = (EventCodes)photonEvent.Code;
            // this is the data of our player
            object[] data = (object[])photonEvent.CustomData;

            Debug.Log("Recevied event " + theEvent);
            switch (theEvent)
            {
                case EventCodes.NewPlayer:
                    NewPlayerRecieve(data);
                    break;
                case EventCodes.ListPlayers:
                    ListPlayerRecieve(data);
                    break;
                case EventCodes.UpdateStat:
                    UpdateStateRecieve(data);
                    break;
            }
        }
    }

    //    // onenable and onDisable 
    //    //is a builtin fun by unity
    //    //using to detect when gameObject
    //    //or component or every
    //    //thing in unity is enabled or disabled



    //    //when an event happen make this script listen to this event
    public override void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }
    //    //when an event happen make this script Remove to this event
    public override void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    //    // we want to detect the updates on the game
    //    // and whether a new player is added or not

    //    //==========================================//
    //    // these two fun related to one player whether it is raised to the network and detect it in the game


    //    //this make the data of the new player raised to the network
    public void NewPlayerSend(string username)
    {
        //this represent the data of the player
        object[] package = new object[3];
        package[0] = username;
        package[1] = PhotonNetwork.LocalPlayer.ActorNumber;
        //this refer to the score;
        package[2] = 0;


        PhotonNetwork.RaiseEvent(
            (byte)EventCodes.NewPlayer,
            package,
            new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient },
            new SendOptions { Reliability = true }
            );
    }

    //    //this recieve the info of the new player 
    public void NewPlayerRecieve(object[] dataReceived)
    {
        PlayerInfo player = new PlayerInfo((string)dataReceived[0], (int)dataReceived[1], (int)dataReceived[2]);
        //first all the player name will appear to the master and we will edit it
        //the master is the one who first create the room
        allPlayers.Add(player);
        ListPlayerSend();
    }
    //    //==========================================//


    //    //==========================================//

    //    //these two fun related to the whole list of players 

    //    //this make all of the player know what is the other players in the game

    public void ListPlayerSend()
    {
        object[] package = new object[allPlayers.Count + 1];

        package[0] = state;
        for (int i = 0; i < allPlayers.Count; i++)
        {
            object[] piece = new object[3];
            piece[0] = allPlayers[i].name;
            piece[1] = allPlayers[i].actor;
            piece[2] = allPlayers[i].score;


            package[i + 1] = piece;
        }
        PhotonNetwork.RaiseEvent(
        (byte)EventCodes.ListPlayers,
        package,
        new RaiseEventOptions { Receivers = ReceiverGroup.All },
        new SendOptions { Reliability = true }
        );

    }

    //    // here we recieve the data of all the players
    public void ListPlayerRecieve(object[] dataReceived)
    {
        allPlayers.Clear();
        state = (GameState)dataReceived[0];
        for (int i = 1; i < dataReceived.Length; i++)
        {
            object[] piece = (object[])dataReceived[i];

            PlayerInfo player = new PlayerInfo(
                (string)piece[0],
                (int)piece[1],
                (int)piece[2]
                );

            allPlayers.Add(player);
            if (PhotonNetwork.LocalPlayer.ActorNumber == player.actor)
            {
                index = i - 1;
            }
        }
        stateCheck();

    }
    //    //==========================================//

    //    // we want to know which player we get info about
    //    //we want to know the score
    public void UpdateStatesSend(int actorSending, int score)
    {
        object[] package = new object[] { actorSending, score };
        PhotonNetwork.RaiseEvent(
          (byte)EventCodes.UpdateStat,
          package,
          new RaiseEventOptions { Receivers = ReceiverGroup.All },
          new SendOptions { Reliability = true }
          );
    }
    public void UpdateStateRecieve(object[] dataReceived)
    {
        int actor = (int)dataReceived[0];
        int amount = (int)dataReceived[1];

        for (int i = 0; i < allPlayers.Count; i++)
        {
            if (allPlayers[i].actor == actor)
            {
                allPlayers[i].score = amount;
                Debug.Log("player " + allPlayers[i].name + allPlayers[i].score);

                if (i == index)
                {
                    UpdateStatsDisplay();
                }

                // because when the leaderBoard is appeared there is no updates or changes
                //we want to update the data when the leaderBoard is appeared too
                if (UIController.instance.leaderboard.activeInHierarchy)
                {
                    showLeaderBoard();
                }
                break;

            }
        }
        ScoreCheck();
    }



    //    //to display the score Value
    public void UpdateStatsDisplay()
    {
        if (allPlayers.Count > index)
        {
            UIController.instance.scoreText.text = "Score: " + allPlayers[index].score;
        }
        else
        {
            UIController.instance.scoreText.text = "Score: 0";

        }
    }

    void showLeaderBoard()
    {
        UIController.instance.leaderboard.SetActive(true);

        foreach (LeaderBoard lp in lboardplayers)
        {
            Destroy(lp.gameObject);
        }
        lboardplayers.Clear();

        UIController.instance.leaderboardPlayerDisplay.gameObject.SetActive(false);

        //sort the players according to their score
        List<PlayerInfo> sorted = SortPlayers(allPlayers);

        // show the leaderBoard UI with the score of each player

        foreach (PlayerInfo player in sorted)
        {
            LeaderBoard newPlayerDisplay = Instantiate(UIController.instance.leaderboardPlayerDisplay, UIController.instance.leaderboardPlayerDisplay.transform.parent);

            newPlayerDisplay.SetDetails(player.name, player.score);

            newPlayerDisplay.gameObject.SetActive(true);

            lboardplayers.Add(newPlayerDisplay);
        }

        return;
    }


    //    // sort players according to their score
    private List<PlayerInfo> SortPlayers(List<PlayerInfo> players)
    {
        List<PlayerInfo> sorted = new List<PlayerInfo>();

        // the sorted list is another thing with the list of players
        while (sorted.Count < players.Count)
        {
            int highes = -1;
            PlayerInfo selectedPlayer = players[0];

            foreach (PlayerInfo player in players)
            {
                /*
                    to check if the player of the highest score
                    in the list do not add it again
                */
                if (!sorted.Contains(player))
                {
                    if (player.score > highes)
                    {
                        selectedPlayer = player;
                        highes = player.score;
                    }
                }

            }

            sorted.Add(selectedPlayer);
        }

        return sorted;
    }


    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        SceneManager.LoadScene(0);
    }
    void ScoreCheck()
    {
        bool winnerFound = false;


        foreach (PlayerInfo player in allPlayers)
        {
            if (player.score >= scoreToWin && scoreToWin > 0)
            {
                winnerFound = true;
                break;
            }
        }

        if (winnerFound)
        {
            if (PhotonNetwork.IsMasterClient && state != GameState.Ending)
            {
                state = GameState.Ending;
                ListPlayerSend();
            }
        }
    }
    //    //this fun check if the state is Ending or not
    void stateCheck()
    {
        if (state == GameState.Ending)
        {
            EndGame();
        }
    }
    void EndGame()
    {
        state = GameState.Ending;
        UIController.instance.endScreen.gameObject.SetActive(true);
        //if (PhotonNetwork.IsMasterClient)
        //{
        //    //this will remove all the players on the game and from the server
        //    PhotonNetwork.DestroyAll();
        //}



        showLeaderBoard();


        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;


        StartCoroutine(EndCo());

    }


    private IEnumerator EndCo()
    {
        //wait for seconds and the move to the map screen
        yield return new WaitForSeconds(waiitAfterEnding);
        //PhotonNetwork.AutomaticallySyncScene = false;
        //PhotonNetwork.LeaveRoom();
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel(6);

        }


    }


}
//// we need an information that the GamePlayManager keep track off and what the game state is
//// we need to store the information of the player


//[System.Serializable]
//public class PlayerInfo
//{
//    public string name;
//    // actor refer to the number assigned to the network player
//    public int actor, score;
//    public PlayerInfo(string nameP, int actorP, int scoreP = 0)
//    {
//        name = nameP;
//        // actor is the number that the network give it to the player
//        actor = actorP;

//        score = scoreP;
//    }

//}

//// Ì⁄‰Ï „„ﬂ‰ ⁄·‘«‰ «Õ›Ÿ «·œ« « ··œ« « »Ì“ «Õ›ŸÂ« ⁄·Ï ÿÊ· „‰ Œ·«· ·Ì”  «·»·«Ì—“
