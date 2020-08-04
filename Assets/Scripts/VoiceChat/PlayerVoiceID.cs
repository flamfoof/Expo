using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Byn.Awrtc;

public class PlayerVoiceID : MonoBehaviourPunCallbacks
{
    public int id;
    public ConnectionId remoteUserID;
    
    private void Start() {
        id = PhotonNetwork.LocalPlayer.ActorNumber;
        remoteUserID = IgniteGameManager.voiceManager.webRTC.GetConnectionId();
        photonView.RPC("SetVoiceID", RpcTarget.AllBuffered, id);
    }
    
    [PunRPC]
    void SetVoiceID(int voiceID)
    {   
        this.id = voiceID;
        //this.remoteUserID = remoteID;
    }
}
