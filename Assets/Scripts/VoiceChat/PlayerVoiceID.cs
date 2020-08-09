using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Byn.Awrtc;
using Byn.Awrtc.Unity;

public class PlayerVoiceID : MonoBehaviourPunCallbacks
{
    public int id;

    public ConnectionId remoteUserID;
    
    private void Start() {
        id = photonView.Owner.ActorNumber;
        
        Debug.Log("ID IS: " + id);
        remoteUserID = IgniteGameManager.voiceManager.webRTC.GetConnectionId();
        //photonView.RPC("SetVoiceID", RpcTarget.AllBuffered, id);
    }
    
    [PunRPC]
    public void SetVoiceID(int voiceID)
    {           
        this.id = voiceID;
        //this.remoteUserID = remoteID;
    }
}
