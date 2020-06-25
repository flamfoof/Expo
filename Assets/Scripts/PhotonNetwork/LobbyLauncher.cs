using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

[RequireComponent(typeof(LobbyLauncherUI))]
public class LobbyLauncher : MonoBehaviourPunCallbacks
{
     /******************************************************
     * Refer to the Photon documentation and scripting API for official definitions and descriptions
     * 
     * Documentation: https://doc.photonengine.com/en-us/pun/current/getting-started/pun-intro
     * Scripting API: https://doc-api.photonengine.com/en/pun/v2/index.html
     * 
     * If your Unity editor and standalone builds do not connect with each other but the multiple standalones
     * do then try manually setting the FixedRegion in the PhotonServerSettings during the development of your project.
     * https://doc.photonengine.com/en-us/realtime/current/connection-and-authentication/regions
     *
     * ******************************************************/
    
    private LobbyLauncherUI lobbyUI;
    public string scene = "Exhibit";
    public string username;
    public string nickname;
    public string password;
    public bool validUsername = false;
    public bool validPassword = false;
    private bool isConnecting = false;
    private string gameVersion = "1.0";
    public byte maxPlayerCount = 10;
    public GameObject playerInfo;

    private void Awake() 
    {
        lobbyUI = GetComponent<LobbyLauncherUI>();
        if(!playerInfo)
        {
            playerInfo = GameObject.FindObjectOfType<AssignPlayerAvatar>().gameObject;
        }
        //syncs the scene when the player loads in to the master scene/room
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    private void Start()
    {
        lobbyUI.UIControls.feedbackText.text = "";
        Debug.Log("Start connecting");
    }

    public void Connect()
    {
        Debug.Log("Connecting to server");
        lobbyUI.UIControls.feedbackText.text = "";

        isConnecting = true;

        CheckIfValidUsername();

        Debug.Log("You have signed in as: " + PhotonNetwork.NickName);
        //hide the button
        lobbyUI.UIControls.submitLoginButton.gameObject.SetActive(false);

        //play the idle loading animation
        lobbyUI.PlayLoadAnimation(true);

        //set the player info
        playerInfo.GetComponent<AssignPlayerAvatar>().Gender = lobbyUI.CheckGender(lobbyUI.UIControls.genderList);

        if(PhotonNetwork.IsConnected)
        {
            LogFeedback("Joining Room...");

            PhotonNetwork.JoinRandomRoom();
        } else {
            LogFeedback("Conecting to server...");
            Debug.Log("Connecting to master server...");

            PhotonNetwork.GameVersion = this.gameVersion;
            PhotonNetwork.ConnectUsingSettings(); //Connects to Photon master servers
            //Other ways to make a connection can be found here: https://doc-api.photonengine.com/en/pun/v2/class_photon_1_1_pun_1_1_photon_network.html
        }
    }

    void LogFeedback(string message)
    {
        // we do not assume there is a feedbackText defined.
        if (lobbyUI.UIControls.feedbackText == null) {
            return;
        }

        // add new messages as a new line and at the bottom of the log.
        lobbyUI.UIControls.feedbackText.text += System.Environment.NewLine+message;
    }

    public override void OnConnectedToMaster()
    {
        if(isConnecting)
        {
            LogFeedback("We are now connected to the " + PhotonNetwork.CloudRegion + " server!");
            Debug.Log("We are now connected to the " + PhotonNetwork.CloudRegion + " server!");

            PhotonNetwork.JoinRandomRoom();
        }
        
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        LogFeedback("Unable to join a random room: Creating a new Space");
        Debug.Log("Unable to join a random room: Creating a new Space");

        //unable to join a random room, so just make one because there are none at the moment.
        PhotonNetwork.CreateRoom(null, new Photon.Realtime.RoomOptions{MaxPlayers = this.maxPlayerCount});
    }
/*
    private void OnDisconnected(DisconnectCause info) 
    {
        LogFeedback("<Color=Red>OnDisconnected</Color> " + info);
        Debug.LogError("Disconnected from server");

        lobbyUI.PlayLoadAnimation(false);

        isConnecting = false;
        lobbyUI.UIControls.submitLoginButton.gameObject.SetActive(true);
    }*/
    
    public override void OnJoinedRoom()
    {
        LogFeedback("<Color=Green>OnJoinedRoom</Color> with "+PhotonNetwork.CurrentRoom.PlayerCount+" Player(s)");
        Debug.Log("<Color=Green>OnJoinedRoom</Color> with "+PhotonNetwork.CurrentRoom.PlayerCount+" Player(s)");

        Debug.Log("Player count: " + PhotonNetwork.CurrentRoom.PlayerCount);
        Debug.Log("Loading scene: " + scene);
        //Only load if you are the first player, else 
        //PhotonNetwork.AutomaticallySyncScene to sync our instance scene.
        if(PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            //call the server to load the scene
            
            PhotonNetwork.LoadLevel(scene);
        }
    }

    public bool CheckIfValidUsername()
    {
        if(!validUsername)
        {
            lobbyUI.UIControls.feedbackText.text = "Invalid username" + 
            "\n Your name must be between 2-12 characters in length";
            return false;
        } else {
            PlayerPrefs.SetString(LobbyLauncherUI.playerNickname, nickname);
            Debug.Log(PlayerPrefs.GetString(LobbyLauncherUI.playerNickname));
            PhotonNetwork.NickName = nickname;
            return true;
        }
    }
}
