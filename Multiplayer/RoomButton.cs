using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Realtime;
public class RoomButton : MonoBehaviour
{
    public TMP_Text buttonText;


    //this store information about the room
    private RoomInfo info;
    // Start is called before the first frame update


    public void SetButtonDetails(RoomInfo inputInfo)
    {
        info = inputInfo;
        buttonText.text = info.Name;
    }
  

    // in this script i pass the room info to the launcher script
    public void OpenRoom()
    {
        Launcher.instance.JoinRoom(info);
    }

   
}
