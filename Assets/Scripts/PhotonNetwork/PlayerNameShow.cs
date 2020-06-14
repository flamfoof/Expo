using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerNameShow : MonoBehaviourPunCallbacks
{
    GameObject local;
    int actorNumber; 
    // Start is called before the first frame update
    void Start()
    {
        //string name = transform.parent.GetComponent
        GetComponent<TextMesh>().text = PhotonNetwork.NickName;
        actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
        //local = photonView.gameObject.name;
        //Debug.Log("actor name: " + photonView.gameObject.name);
        Debug.Log("actor number: " + PhotonNetwork.LocalPlayer.ActorNumber + " name: " + PhotonNetwork.NickName);
        //local = PhotonNetwork.CurrentRoom.GetPlayer(actorNumber);
        //local = (GameObject)PhotonNetwork.LocalPlayer.TagObject;
        Debug.Log(PhotonNetwork.LocalPlayer.TagObject);
        
    }

    void FixedUpdate()
    {
        
    }
}
