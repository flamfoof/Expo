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
                
                PhotonNetwork.Instantiate(this.playerPrefab.name, spawnLoc.transform.position, spawnLoc.transform.rotation, 0);
            } else {
                Debug.LogFormat("Ignoring scene load for {0}", SceneManagerHelper.ActiveSceneName);
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
			{
				QuitApplication();
			}
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("Player has entered the room" + newPlayer.NickName);

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
}
