using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;


public class IgniteGameManager : MonoBehaviourPunCallbacks
{
    public GameObject playerPrefab;
    static public IgniteGameManager IgniteInstance;
    public GameObject spawnLoc;
    public string sceneLogin = "Login";
    public string sceneMain = "Exhibit";

    private GameObject instance;

    void Start()
    {
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
                
                GameObject spawnedPlayer = PhotonNetwork.Instantiate(this.playerPrefab.name, spawnLoc.transform.position, spawnLoc.transform.rotation, 0);
                spawnedPlayer.GetComponent<FirstPersonAIO>().enabled = true;
                spawnedPlayer.GetComponent<FirstPersonAIO>().playerCamera.gameObject.SetActive(true);                
                spawnedPlayer.GetComponent<FirstPersonAIO>().playerCamera.gameObject.transform.localPosition = spawnedPlayer.GetComponent<FirstPersonAIO>().cameraOrigin.transform.localPosition;
                
                ServerAvatarChange(spawnedPlayer);

            } else {
                Debug.LogFormat("Ignoring scene load for {0}", SceneManagerHelper.ActiveSceneName);
            }
        }
        InvokeRepeating("RefreshAvatarList", 1.0f, 2.0f);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            QuitApplication();
        }
    }

    void RefreshAvatarList()
    {
        //update the meshes
        //sorry it's messy!
        Debug.Log("Refreshing avatar");
        AssignPlayerAvatar changeAvatar = GameObject.FindObjectOfType<AssignPlayerAvatar>();

        int count = 0;
        foreach(PhotonView pv in GameObject.FindObjectsOfType(typeof(PhotonView)))
        {
            Player pl = pv.Owner;
            if(pv.gameObject.GetComponent<UserActions>())
            {
                if( pv.GetComponent<AttachAvatar>().avatarBodyLocation.GetComponent<AvatarInfo>().meshHair.sharedMesh == 
                    changeAvatar.defaultPrefab.GetComponent<AvatarInfo>().meshHair.sharedMesh)
                {
                    Debug.Log(pv.Owner.NickName + " has selected their character: " + 
                        (GenderList.genders)PhotonNetwork.PlayerList[count].CustomProperties["AvatarType"]);
                    changeAvatar.ChangeAvatar(pv.gameObject.GetComponent<AttachAvatar>().avatarBodyLocation.GetComponent<AvatarInfo>(), 
                        (GenderList.genders)PhotonNetwork.PlayerList[count].CustomProperties["AvatarType"]);
                }
                count++;
            }            
        }
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
    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene(sceneLogin);
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
}
