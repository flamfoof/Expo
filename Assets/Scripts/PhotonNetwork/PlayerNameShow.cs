using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerNameShow : MonoBehaviourPunCallbacks
{
    GameObject localPlayer;
    IgniteGameManager gm;
    public GameObject playerName;
    public GameObject playerOrganization;
    public GameObject infoUI;
    public bool infoUIEnabled;
    int actorNumber; 

    // Start is called before the first frame update
    void Start()
    {
        localPlayer = IgniteGameManager.localPlayer;
        gm = IgniteGameManager.IgniteInstance;
        
        foreach (PhotonView pv in gm.GetPlayerList())
        {
            //Show the name and organization from PlayerPrefs
            //pv.GetComponent<UserActions>().playerName.text = pv.Owner.NickName;
            //pv.GetComponent<UserActions>().playerOrganization.text = PlayerPrefs.GetString("Organization", "");
        }
    }

    void Update()
    {
        transform.LookAt(localPlayer.transform, Vector3.up);

        /*
        foreach(PhotonView pv in gm.GetPlayerList())
        {
            if(!pv.IsMine)
            {
                try{    
                    pv.GetComponent<UserActions>().playerName.transform.LookAt(IgniteGameManager.localPlayer.transform, Vector3.up);
                    pv.GetComponent<UserActions>().playerOrganization.transform.LookAt(IgniteGameManager.localPlayer.transform, Vector3.up);
                }
                catch (Exception e) {
                    Debug.Log("Exception when finding aiming name: " + e);
                    Debug.Log("player in player list not found");
                    gm.RefreshPlayerList();
                }
            }
        }*/
    }

    public void EnablePlayerButtonInfoUI()
    {
        infoUI.SetActive(true);
        infoUIEnabled = true;
        playerName.SetActive(false);
        playerOrganization.SetActive(false);
    }

    public void DisablePlayerButtonInfoUI()
    {
        infoUI.SetActive(false);
        infoUIEnabled = false;
        playerName.SetActive(true);
        playerOrganization.SetActive(true);
    }
}
