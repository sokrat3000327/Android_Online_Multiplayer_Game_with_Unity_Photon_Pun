using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class controller4 : MonoBehaviourPunCallbacks
{
    //rotation
    public Transform viewPoint;
    public float mouseSensitivity = 1f;
    private float verticalRotStore;
    private Vector2 mouseInput;

    public bool invertLook;
    private int myScore;
    private GameManager2 GM;

    //======================



    //movement

    public float moveSpeed = 5f;
    private Vector3 moveDir, movement;


    /*Character Controller is a builtin component which all player to move
     * and not to pass through the walls and to walk over the slope
     * */
    
    public CharacterController charCon;

    //======================


    //Camera

    private Camera cam;


    //======================

    //Gravity
    private float yVel;



    // Start is called before the first frame update
    void Start()
    {
        // confined: to make the mouse look around only the game screen



        Cursor.lockState = CursorLockMode.Confined;

        cam = Camera.main;
        //Transform newTrans = SpawnManager.instance.GetSpawnPoint();
        //transform.position = newTrans.position;
        //transform.rotation = newTrans.rotation;
        GM = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<GameManager2>();



    }

    // Update is called once per frame
    void Update()
    {
        //Is Mine means that the player control it self
        if (photonView.IsMine)
        {
            //    //to control the rotation of the player
            mouseInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y")) * mouseSensitivity;

            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y + mouseInput.x, transform.rotation.eulerAngles.z);

            //To control when to look up and down
            verticalRotStore += mouseInput.y;
            verticalRotStore = Mathf.Clamp(verticalRotStore, -60f, 60f);
            
            if (invertLook)
            {
                viewPoint.rotation = Quaternion.Euler(verticalRotStore, viewPoint.rotation.eulerAngles.y, viewPoint.rotation.eulerAngles.z);
            }
            else
            {
                viewPoint.rotation = Quaternion.Euler(-verticalRotStore, viewPoint.rotation.eulerAngles.y, viewPoint.rotation.eulerAngles.z);

            }



            //control movement

            moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));


            float yVel = movement.y;
            movement = ((transform.forward * moveDir.z) + (transform.right * moveDir.x)).normalized;
            movement.y = yVel;

            if (charCon.isGrounded)
            {
                movement.y = 0f;
            }


            movement.y = Physics.gravity.y * Time.deltaTime;

            charCon.Move(movement * moveSpeed * Time.deltaTime);

            //end code of movement

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Cursor.lockState = CursorLockMode.None;
            } else if (Cursor.lockState == CursorLockMode.None)
            {

                // 0 refer to left click mouse
                //1 right click  
                //2 middle mouse click
                if (Input.GetMouseButtonDown(0))
                {
                    Cursor.lockState = CursorLockMode.Confined;
                }

            }

            myScore = GM.total;
            GamePlayManager4.instance.UpdateStatesSend(PhotonNetwork.LocalPlayer.ActorNumber, myScore);
        }


    }

    // for every frame execute the update fun then execute LateUpdate
    private void LateUpdate()
    {
        if (photonView.IsMine)
        {
            cam.transform.position = viewPoint.position;
            cam.transform.rotation = viewPoint.rotation;
        }



    }
}
