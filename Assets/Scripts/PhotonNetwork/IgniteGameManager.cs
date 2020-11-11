using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Byn.Unity.Examples;

public class IgniteGameManager : MonoBehaviourPunCallbacks
{
    public GameObject playerPrefab;
    static public IgniteGameManager IgniteInstance;
    static public CommunicationManager voiceManager;
    public int playerVoiceID;
    public CommandRing commandUI;
    public List<PhotonView> playerList;
    public List<int> uniquePlayersLogged;
    public int totalUniquePlayers;
    static public GameObject localPlayer;
    public GameObject spawnLoc;
    public string sceneLogin = "Login";
    public string sceneMain = "Exhibit";
    private AssignPlayerAvatar changeAvatar;
    public bool gameTesting = false;

    private GameObject instance;

    public IgniteAnalytics analyticsBoard;

    public GameObject handStateObj;

    void Start()
    {
        playerList = new List<PhotonView>();
        IgniteInstance = this;

        if(gameTesting)
        {
            Application.logMessageReceived += CustomLogger;
        }
        if(!voiceManager)
        {
            voiceManager = GameObject.FindObjectOfType<CommunicationManager>();
        }
        
        if(!PhotonNetwork.IsConnected)
        {
            SceneManager.LoadScene(sceneLogin);            
            return;
        }
        if(!playerPrefab)
        {
            Debug.LogError("We need the player prefab in this game object");
        } else {
            if(!IgnitePlayerManager.LocalPlayerInstance)
            {
                SpawnPlayer();
            } else {
                Debug.LogFormat("Ignoring scene load for {0}", SceneManagerHelper.ActiveSceneName);
            }
        }

        if(AnalyticsController.Instance)
        {
            AnalyticsController.Instance.ProfileInfoAnalytics();
        }

        if (SessionHandler.instance.CheckIfPresenter())
        {
            GetComponent<OneToMany>().sAddress = SessionHandler.instance.passAdress;
            GetComponent<OneToMany>().StartStream();
        }

        commandUI.EnableStreamBtns(SessionHandler.instance.CheckIfPresenter());
    }

    private void SpawnPlayer()
    {
        Debug.LogFormat("We are Instantiating LocalPlayer from {0}", SceneManagerHelper.ActiveSceneName);
        Player player;
        GameObject spawnedPlayer = PhotonNetwork.Instantiate(this.playerPrefab.name, spawnLoc.transform.position, spawnLoc.transform.rotation, 0);
        player = spawnedPlayer.GetPhotonView().Owner;
        changeAvatar = GameObject.FindObjectOfType<AssignPlayerAvatar>();        
        Hashtable hash = new Hashtable();
        FirstPersonAIO FPAIO = spawnedPlayer.GetComponent<FirstPersonAIO>();

        FPAIO.enabled = true;
        FPAIO.playerCamera.gameObject.SetActive(true);                
        FPAIO.playerCamera.gameObject.transform.localPosition = FPAIO.cameraOrigin.transform.localPosition;
        spawnedPlayer.GetComponent<UserActions>().playerName.text = player.NickName;

        if(spawnedPlayer.GetComponent<PhotonView>().IsMine)
        {
            localPlayer = spawnedPlayer;
        }

        PhotonNetwork.LocalPlayer.SetCustomProperties(hash); 
        player = spawnedPlayer.GetPhotonView().Owner;

        //In the AttachAvatar.cs
        //spawnedPlayer.GetPhotonView().RPC("SetPlayerCustomization", RpcTarget.AllBuffered, player.ActorNumber);
        StartCoroutine(RefreshAvatars(2.0f));
        
        #if UNITY_WEBGL
        if(voiceManager)
            voiceManager.webRTC.SendMsg(player.NickName + " has joined the room.");
        #endif
    }

    /// <summary>
    /// Called from UserAction. When Player prefab is spawned, force a refresh of the player list and WebRTC connections.
    /// </summary>
    public void RefreshOnPlayerSpawn()
    {
        RefreshPlayerList();

        if(voiceManager)
            voiceManager.RefreshWebGLSpeakers();        
    }

    void PrintRoomName()
    {
        Debug.Log("Room - Client state: " + PhotonNetwork.NetworkClientState.ToString());
        Debug.Log("Room - Server: " + PhotonNetwork.Server.ToString());
        Debug.Log(PhotonNetwork.CurrentRoom != null ? PhotonNetwork.CurrentRoom.Name : "Not Joined");
    }

    void RefreshAllPlayerAvatar()
    {
        for(int i = 0; i < playerList.Count; i++)
        {
            AttachAvatar currentPVAvatar = playerList[i].GetComponent<AttachAvatar>();
            GenderList.genders gender = (GenderList.genders)((int)playerList[i].Owner.CustomProperties["AvatarGender"]);
            int indexSuit = (int)playerList[i].Owner.CustomProperties["AvatarBodyIndex"];
            int indexHead = (int)playerList[i].Owner.CustomProperties["AvatarHeadIndex"];
            Debug.Log("Avatar - head: " + indexHead + " body: " + indexSuit + " gender: " + gender);
            
            if(gender == GenderList.genders.Male1)
            {
                currentPVAvatar.avatarInfo.maleAvatar.SetActive(true);
                currentPVAvatar.avatarInfo.femaleAvatar.SetActive(false);
                currentPVAvatar.avatarInfo.anim = currentPVAvatar.avatarInfo.maleAvatar.GetComponent<Animator>();
                currentPVAvatar.avatarInfo.maleAvatar.GetComponent<scr_Selector>().pickOneSuit(indexSuit);
            } else
            {
                currentPVAvatar.avatarInfo.maleAvatar.SetActive(false);
                currentPVAvatar.avatarInfo.femaleAvatar.SetActive(true);
                currentPVAvatar.avatarInfo.anim = currentPVAvatar.avatarInfo.femaleAvatar.GetComponent<Animator>();
            }       
        }
    }

    #region Photon Network Functions
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("<color=green>Player has entered the room: </color>" + newPlayer.NickName);
        
        if(voiceManager.webRTC.gameObject.activeSelf)
        {
            StartCoroutine(voiceManager.webRTC.ReconnectToPlayerVoice(newPlayer.ActorNumber));
            voiceManager.webRTC.SendMsg(newPlayer.NickName + " has joined the room.");
            voiceManager.webRTC.ReconnectAllVoiceID();
        }
        
        StartCoroutine(RefreshAvatars(2.0f));
        
        if(PhotonNetwork.IsMasterClient)
        {
            Debug.LogFormat( "OnPlayerEnteredRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient ); // called before OnPlayerLeftRoom
        }        
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log("Player has left: " + otherPlayer.NickName);

        if(PhotonNetwork.IsMasterClient)
        {
            Debug.LogFormat( "OnPlayerEnteredRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient ); // called before OnPlayerLeftRoom
        }
        
        PhotonNetwork.DestroyPlayerObjects(otherPlayer);

        RefreshPlayerList();

        if(voiceManager)
            voiceManager.RefreshWebGLSpeakers();
    }

    public override void OnLeftRoom()
    {
        StartCoroutine(UnfocusApplicationCursor());
        SceneManager.LoadScene(sceneLogin);
    }
    #endregion

    /// <summary>
    /// Changes the cursor state to Unfocused after next Fixed Update
    /// </summary>
    public IEnumerator UnfocusApplicationCursor()
    {
        yield return new WaitForFixedUpdate();

        Cursor.lockState = CursorLockMode.None; 
        Cursor.visible = true;
    }

    /// <summary>
    /// In case of incorrect players in the list, use this to find all the players in the room.
    /// </summary>
    public void RefreshPlayerList()
    {
        playerList.Clear();

        foreach(PhotonView pv in GameObject.FindObjectsOfType(typeof(PhotonView)))
        {
            if(pv) //checks if they are still connected
            {
                if(pv.gameObject.GetComponent<UserActions>())
                {
                    playerList.Add(pv);
                    pv.GetComponent<UserActions>().playerName.text = pv.Owner.NickName;
                }      
            }
        }        
    }

    /// <summary>
    /// Returns the list of current players in the Photon room.
    /// </summary>
    public List<PhotonView> GetPlayerList()
    {
        return this.playerList;
    }

    /// <summary>
    /// Current solution to the avatars spawning with their current settings set by themselves.
    /// </summary>
    public IEnumerator RefreshAvatars(float delay)
    {                 
        yield return new WaitForSeconds(delay);
        
        for(int i = 0; i < playerList.Count; i++)
        {            
            playerList[i].GetComponent<AttachAvatar>().SetPlayerCustomization(playerList[i].OwnerActorNr);

        }
    }

    /// <summary>
    /// Calculates the number of unique players in a room based on their Photon Nicknames. Called when new players enter.
    /// </summary>
    public void RefreshUniquePlayer()
    {
        foreach(PhotonView pv in GameObject.FindObjectsOfType(typeof(PhotonView)))
        {
            if(pv.GetComponent<UserActions>())
            {
                /*
                if(!uniquePlayersLogged.Contains(pv.Owner.NickName))
                {
                    uniquePlayersLogged.Add(pv.Owner.NickName);
                    Debug.Log("New unique player has joined: " + pv.Owner.NickName);
                    if(PhotonNetwork.IsMasterClient)
                    {
                        totalUniquePlayers++;

                        if(AnalyticsController.Instance)
                        {
                            AnalyticsController.Instance.AttendesNumber(totalUniquePlayers);
                        }
                    }
                    
                } */
                if(!uniquePlayersLogged.Contains(pv.OwnerActorNr))
                {
                    //doesn't work, just take the highest actor number
                    
                    //uniquePlayersLogged.Add(pv.OwnerActorNr);
                    Debug.Log("Being logged is: " + pv.Owner.ActorNumber);
                    //totalUniquePlayers++;
                    Debug.Log("Logged unique player: " + totalUniquePlayers);
                    if(totalUniquePlayers < pv.OwnerActorNr)
                        totalUniquePlayers = pv.OwnerActorNr;
                    if(analyticsBoard)
                    {
                        analyticsBoard.GetComponent<BBBAnalytics>().UpdateAttendeesCount(totalUniquePlayers);
                    }
                    
                    if(AnalyticsController.Instance)
                    {
                        AnalyticsController.Instance.AttendesNumber(totalUniquePlayers);
                    } 
                }                
            }                  
        }
    }

    void CustomLogger(string logString, string stackTrace, LogType type)
    {
        
        if(type == 0)
        {
            Debug.Log("Custom Logging");
            Debug.Log(logString);
            voiceManager.webRTC.SendMsg(logString);
        }
        
        //Debug.Log(stackTrace);
        //Debug.Log(type.ToString());
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public void QuitApplication()
    {
        Application.Quit();
    }
}
