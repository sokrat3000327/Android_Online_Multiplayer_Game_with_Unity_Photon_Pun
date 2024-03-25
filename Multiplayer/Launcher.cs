using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//to include photon function
using Photon.Pun;
using TMPro;
using Photon.Realtime;


public class Launcher : MonoBehaviourPunCallbacks
{
    public static Launcher instance;

    public GameObject loadingScreen;
    public TMP_Text loadingText;
    public GameObject menuButtons;
    public GameObject createRoomScreen;
    public TMP_InputField roomNameInput;
    public GameObject roomScreen;
    public TMP_Text roomNameText , playerNameLabel;

    // list of all players
    private List<TMP_Text> allPlayerNames = new List<TMP_Text>();

    public GameObject errorScreen;
    public TMP_Text errorText;

    public GameObject roomBrowserScreen;
    public GameObject nameInputScreen;
    public TMP_InputField nameInput;

    public string levelToPlay;

    public GameObject startButton;
    public GameObject RoomTestButton;

    //to check that the nickName do not occur multiple time
    public static bool hasSetNick;
    public string[] allMaps;
    public bool changeMapBetweenRounds = true;



    public RoomButton theRoomButton;
    private List<RoomButton> allRoomButtons = new List<RoomButton>();
    private Dictionary<string, RoomInfo> cachedRoomsList = new Dictionary<string, RoomInfo>();

    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        CloseMenu();

        loadingScreen.SetActive(true);
        loadingText.text = "Connecting To Network....";

        //this mean connect to Network
        PhotonNetwork.ConnectUsingSettings();

// this fun will only execute in the editor only , not in the build
#if UNITY_EDITOR
        RoomTestButton.SetActive(true);
#endif


        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    //==========UI Scripts===============//
    void CloseMenu()
    {
        loadingScreen.SetActive(false);
        menuButtons.SetActive(false);
        createRoomScreen.SetActive(false);
        roomScreen.SetActive(false);
        errorScreen.SetActive(false);
        roomBrowserScreen.SetActive(false);
        nameInputScreen.SetActive(false);
    }
    public void CloseErrorScreen()
    {
        CloseMenu();
        menuButtons.SetActive(true);
    }
    public override void OnLeftRoom()
    {
        CloseMenu();
        menuButtons.SetActive(true);
    }
    public void OpenRoomBrowser()
    {
        CloseMenu();
        roomBrowserScreen.SetActive(true);
    }
    public void CloseRoomBrowser()
    {
        CloseMenu();
        menuButtons.SetActive(true);
    }


    public void SetNickName()
    {

        //make the player NickName as the the text of the input field
        if (!string.IsNullOrEmpty(nameInput.text))
        {
            PhotonNetwork.NickName = nameInput.text;

            PlayerPrefs.SetString("playerName", nameInput.text);

            CloseMenu();

            menuButtons.SetActive(true);

            hasSetNick = true;
        }
    }


    public void QuitGame()
    {
        Application.Quit();
    }

    public void startGame()
    {
        PhotonNetwork.LoadLevel(2);
    }

    public void QuickJoin()
    {
        RoomOptions options = new RoomOptions();
        options.MaxPlayers = 8;

        


        PhotonNetwork.CreateRoom("Test",options);
        CloseMenu();
        loadingText.text = "Creating Room";
        loadingScreen.SetActive(true);
    }


    
    //==========UI Scripts===============//


    





    // ========photon server scripts ==============//
    //this mean when you connect to the Master Server do....
    public override void OnConnectedToMaster()
    {
        
        //connect to Lobby which allow us to create a room which make all player play on it
        PhotonNetwork.JoinLobby();


        //this able to tell the photon which scene will be loaded
        //control the scene that you are going to
        PhotonNetwork.AutomaticallySyncScene = true;

        loadingText.text = "Joining Lobby....";
    }
    
    //this mean when you connect to lobby do....  


    public override void OnJoinedLobby()
    {
        CloseMenu();
        menuButtons.SetActive(true);

        //make the player names a random numbers
        PhotonNetwork.NickName = Random.Range(0, 1000).ToString();

        // this if for the first time we open the game
        if (!hasSetNick)
        {
            CloseMenu();
            nameInputScreen.SetActive(true);

            if (PlayerPrefs.HasKey("playerName"))
            {
                nameInput.text = PlayerPrefs.GetString("playerName");
            }
        }
        // this else for all other times
        else {
            PhotonNetwork.NickName = PlayerPrefs.GetString("playerName");
        }
    }



    public void OpenRoomCreate()
    {
        CloseMenu();
        createRoomScreen.SetActive(true);
    }
    // this fun create the room which allow multiplayer to join that room like cross fire
    
    public void CreateRoom()
      {
          if(!string.IsNullOrEmpty(roomNameInput.text))
          {
               RoomOptions options = new RoomOptions();
               options.MaxPlayers = 8;
          
               PhotonNetwork.CreateRoom(roomNameInput.text, options);
               CloseMenu();
               loadingText.text = "Creating Room....";
               loadingScreen.SetActive(true);
           }

      }
    // when you join the room do some thing
    public override void OnJoinedRoom()
    {
        CloseMenu();
        roomScreen.SetActive(true);
        roomNameText.text = PhotonNetwork.CurrentRoom.Name;

        ListAllPlayers();

        // this select which is the master of the photon (Game)
        if (PhotonNetwork.IsMasterClient)
        {
            startButton.SetActive(true);
        }
        else {
            startButton.SetActive(false);
        }
    }
    private void ListAllPlayers()
    {
        foreach(TMP_Text player in allPlayerNames)
        {
            Destroy(player.gameObject);
        }
        allPlayerNames.Clear();

        // Player class follow photon 
        // we do not need for loop on the player it will store all the player automatically
        Player[] players = PhotonNetwork.PlayerList;
        
        // this assign the player names and show it
        for (int i=0; i<players.Length; i++)
        {
            TMP_Text newPlayerLabel = Instantiate(playerNameLabel , playerNameLabel.transform.parent);
            newPlayerLabel.text = players[i].NickName;
            newPlayerLabel.gameObject.SetActive(true);

            allPlayerNames.Add(newPlayerLabel);
        }

    }

    // when the player enter the room
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        TMP_Text newPlayerLabel = Instantiate(playerNameLabel, playerNameLabel.transform.parent);
        newPlayerLabel.text = newPlayer.NickName;
        newPlayerLabel.gameObject.SetActive(true);

        allPlayerNames.Add(newPlayerLabel);
    }

    // when the player left the room
    // we want to get informations
    // we will loop through the player list and find which player we can not found
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        ListAllPlayers();
    }

    // when the creation of room is failed
    public override void OnCreateRoomFailed(short returnCode, string message)
     {
        errorText.text = "Failed To Create Room " + message;
        CloseMenu();
        errorScreen.SetActive(true);
     }

 
    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        CloseMenu();
        loadingText.text = "Leaving Room";
        loadingScreen.SetActive(true);
    }




    // when left the room do....

    // this is called when any change happen in the room
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        UpdateCachedRoomList(roomList);

    }
    public void UpdateCachedRoomList(List<RoomInfo> roomList)
    {
        for (int i=0;i< roomList.Count;i++)
        {
            RoomInfo info = roomList[i];
            if (info.RemovedFromList)
            {
                cachedRoomsList.Remove(info.Name);
            }
            else {
                cachedRoomsList[info.Name] = info;
            }
        }
        RoomListButtonUpdate(cachedRoomsList);
       
    }


    void RoomListButtonUpdate(Dictionary<string , RoomInfo> cachedRoomList)
    {
        foreach (RoomButton rb in allRoomButtons)
        {
            Destroy(rb.gameObject);
        }
        

        theRoomButton.gameObject.SetActive(false);
        foreach (KeyValuePair<string , RoomInfo > roomInfo in cachedRoomList)
        {
            RoomButton newButton = Instantiate(theRoomButton, theRoomButton.transform.parent);
            newButton.SetButtonDetails(roomInfo.Value);
            newButton.gameObject.SetActive(true);
            allRoomButtons.Add(newButton);
            Debug.Log(newButton.name);
        }
    }
    public void JoinRoom(RoomInfo inputInfo)
    {
        PhotonNetwork.JoinRoom(inputInfo.Name);

        CloseMenu();
        loadingText.text = "Joining Room";
        loadingScreen.SetActive(true);
    }

    /* if the master start the game , the master is the first one to start the game, if he start the other player
        will can not find the start button
    - we will do here that if the master leave the game the other player will be the master and the start game will
    appear to him

    */
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            startButton.SetActive(true);
        }
        else
        {
            startButton.SetActive(false);
        }
    }
    // ========photon server scripts ==============//
}



