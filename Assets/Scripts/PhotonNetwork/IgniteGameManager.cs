using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;


public class IgniteGameManager : MonoBehaviourPunCallbacks
{
    public GameObject playerPrefab;
    static public IgniteGameManager IgniteInstance;
    static public CommunicationManager voiceManager;
    public int playerVoiceID;
    public CommandRing commandUI;
    public List<PhotonView> playerList;
    public List<string> uniquePlayersLogged;
    public int totalUniquePlayers;
    static public GameObject localPlayer;
    public GameObject spawnLoc;
    public string sceneLogin = "Login";
    public string sceneMain = "Exhibit";
    private AssignPlayerAvatar changeAvatar;
    public bool gameTesting = false;

    private GameObject instance;

    void Start()
    {
        if(gameTesting)
        {
            Application.logMessageReceived += CustomLogger;
        }

        playerList = new List<PhotonView>();
                
        if(!voiceManager)
        {
            voiceManager = GameObject.FindObjectOfType<CommunicationManager>();
        }
        
           

        IgniteInstance = this;

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
                Debug.LogFormat("We are Instantiating LocalPlayer from {0}", SceneManagerHelper.ActiveSceneName);
                Player player;
                GameObject spawnedPlayer = PhotonNetwork.Instantiate(this.playerPrefab.name, spawnLoc.transform.position, spawnLoc.transform.rotation, 0);
                player = spawnedPlayer.GetPhotonView().Owner;
                changeAvatar = GameObject.FindObjectOfType<AssignPlayerAvatar>();        
                Hashtable hash = new Hashtable();                 


                spawnedPlayer.GetComponent<FirstPersonAIO>().enabled = true;
                spawnedPlayer.GetComponent<FirstPersonAIO>().playerCamera.gameObject.SetActive(true);                
                spawnedPlayer.GetComponent<FirstPersonAIO>().playerCamera.gameObject.transform.localPosition = spawnedPlayer.GetComponent<FirstPersonAIO>().cameraOrigin.transform.localPosition;
                spawnedPlayer.GetComponent<UserActions>().playerName.text = player.NickName;
                if(spawnedPlayer.GetComponent<PhotonView>().IsMine)
                {
                    localPlayer = spawnedPlayer;                    
                }

                //ServerAvatarChange(spawnedPlayer);
                hash.Add("AvatarType", changeAvatar.Gender);

                PhotonNetwork.LocalPlayer.SetCustomProperties(hash); 
                player = spawnedPlayer.GetPhotonView().Owner;
            } else {
                Debug.LogFormat("Ignoring scene load for {0}", SceneManagerHelper.ActiveSceneName);
            }
        }

        if(AnalyticsController.Instance)
        {
            AnalyticsController.Instance.ProfileInfoAnalytics();
        }
        
    }

    void PrintRoomName()
    {
        Debug.Log("room name");
        Debug.Log("Client state: " + PhotonNetwork.NetworkClientState.ToString());
        Debug.Log("Server: " + PhotonNetwork.Server.ToString());
        Debug.Log(PhotonNetwork.CurrentRoom != null ? PhotonNetwork.CurrentRoom.Name : "Not Joined");
    }

    //doesn't work. The player instantiates later than when joined room
    void RPCRefreshAvatar()
    {
        AssignPlayerAvatar changeAvatar = GameObject.FindObjectOfType<AssignPlayerAvatar>();        
        PhotonView photonView = PhotonView.Get(changeAvatar.photonView);
        photonView.RPC("RefreshAvatarList", RpcTarget.AllBuffered);
    }


    public void RefreshOnPlayerSpawn()
    {
        RefreshPlayerList();

        if(voiceManager)
            voiceManager.RefreshWebGLSpeakers();
        
        RefreshAvatars();
        Invoke("RefreshAvatars", 3.0f);
    }


    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("Player has entered the room " + newPlayer.NickName);

        if(PhotonNetwork.IsMasterClient)
        {
            Debug.LogFormat( "OnPlayerEnteredRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient ); // called before OnPlayerLeftRoom

            //LoadExpo();
        }
        
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log("Player has left: " + otherPlayer.NickName);

        if(PhotonNetwork.IsMasterClient)
        {
            Debug.LogFormat( "OnPlayerEnteredRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient ); // called before OnPlayerLeftRoom
            
            //LoadExpo();
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

    public IEnumerator UnfocusApplicationCursor()
    {
        yield return new WaitForFixedUpdate();

        Cursor.lockState = CursorLockMode.None; 
        Cursor.visible = true;
    }


    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public void QuitApplication()
    {
        Application.Quit();
    }

    public void LoadExpo()
    {
        if(!PhotonNetwork.IsMasterClient)
        {
            Debug.LogError( "PhotonNetwork : Trying to Load a level but we are not the master Client" );
        }
        
        Debug.LogFormat( "PhotonNetwork : Loading Level : {0}", PhotonNetwork.CurrentRoom.PlayerCount );

        PhotonNetwork.LoadLevel(sceneMain);
    }

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


    public List<PhotonView> GetPlayerList()
    {
        return this.playerList;
    }

    public void SetParent(Transform parent, Transform child)
    {
        child.SetParent(parent);
    }

    public void ServerAvatarChange(GameObject player)
    {
        AssignPlayerAvatar changeAvatar = GameObject.FindObjectOfType<AssignPlayerAvatar>();

        if(player.GetComponent<PhotonView>().IsMine)
        {            
            Debug.Log(player.GetComponent<PhotonView>().Owner.NickName + " has selected their character: " + (GenderList.genders)PhotonNetwork.LocalPlayer.CustomProperties["AvatarType"]);
            changeAvatar.ChangeAvatar(player.GetComponent<AttachAvatar>().avatarBodyLocation.GetComponent<AvatarInfo>(), (GenderList.genders)PhotonNetwork.LocalPlayer.CustomProperties["AvatarType"]);
        } else{
            int count = 0;
            foreach(PhotonView pv in GameObject.FindObjectsOfType(typeof(PhotonView)))
            {
                Player pl = pv.Owner;
                if(pl != null)
                {
                    if(player == pv.gameObject)
                    {
                        Debug.Log(pv.Owner.NickName + " has selected their character: " + (GenderList.genders)PhotonNetwork.LocalPlayer.CustomProperties["AvatarType"]);
                        changeAvatar.ChangeAvatar(player.GetComponent<AttachAvatar>().avatarBodyLocation.GetComponent<AvatarInfo>(), (GenderList.genders)PhotonNetwork.PlayerListOthers[count].CustomProperties["AvatarType"]);
                        player.transform.name = pv.Owner.NickName;
                    }
                }
                count++;
            }
            /*
            for(int i = 0; i < PhotonNetwork.PlayerListOthers.; i++)
            {
                if(player == PhotonNetwork.PlayerListOthers[i].)
                    changeAvatar.ChangeAvatar(player.GetComponent<AttachAvatar>().avatarBodyLocation.GetComponent<AvatarInfo>(), (GenderList.genders)PhotonNetwork.PlayerListOthers[i].CustomProperties["AvatarType"]);
            }*/            
        }
    }

    public void RefreshAvatars()
    {
        //update the meshes
        //sorry it's messy!                   
        int count = 0;
                    
        
        foreach(PhotonView pv in GameObject.FindObjectsOfType(typeof(PhotonView)))
        {
            Player pl = pv.Owner;
            
            if(pv.gameObject.GetComponent<UserActions>())
            {
                if( pv.GetComponent<AttachAvatar>().avatarBodyLocation.GetComponent<AvatarInfo>().meshHead.sharedMesh == 
                    changeAvatar.defaultPrefab.GetComponent<AvatarInfo>().meshHead.sharedMesh)
                {
                    Debug.Log(pv.Owner.NickName + " has selected their character: " + 
                        (GenderList.genders)pv.Owner.CustomProperties["AvatarType"]);
                    changeAvatar.ChangeAvatar(pv.gameObject.GetComponent<AttachAvatar>().avatarBodyLocation.GetComponent<AvatarInfo>(), 
                        (GenderList.genders)pv.Owner.CustomProperties["AvatarType"]);
                }
                count++;
            }            
        }
    }

    public void RefreshUniquePlayer()
    {
        foreach(PhotonView pv in GameObject.FindObjectsOfType(typeof(PhotonView)))
        {
            if(pv.GetComponent<UserActions>())
            {
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
                    
                }  
            }
                  
        }
    }

    public void PrintPlayerStats()
    {
        for(int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            
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
}
