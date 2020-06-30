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

        foreach(PhotonView pv in gm.GetPlayerList())
        {
            pv.GetComponent<UserActions>().playerName.text = pv.Owner.NickName;
        }
        
    }

    void Update()
    {
        foreach(PhotonView pv in gm.GetPlayerList())
        {
            if(!pv.IsMine)
            {
                pv.GetComponent<UserActions>().playerName.transform.LookAt(IgniteGameManager.localPlayer.transform, Vector3.up);    
            }
        }
    }
}
