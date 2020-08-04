using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerNameShow : MonoBehaviourPunCallbacks
{
    GameObject local;
    IgniteGameManager gm;
    int actorNumber; 
    // Start is called before the first frame update
    void Start()
    {
        gm = GameObject.FindObjectOfType<IgniteGameManager>();
        foreach (PhotonView pv in gm.GetPlayerList())
        {
            //Show the name and organization from PlayerPrefs
            //pv.GetComponent<UserActions>().playerName.text = pv.Owner.NickName;
            pv.GetComponent<UserActions>().playerOrganization.text = PlayerPrefs.GetString("Organization", "");
        }
    }

    void Update()
    {
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
        }
    }
}
