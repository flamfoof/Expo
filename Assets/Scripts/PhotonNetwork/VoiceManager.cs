using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if !UNITY_WEBGL
using Photon.Voice;
#endif
using Photon.Pun;
using Photon.Realtime;

public class VoiceManager : MonoBehaviourPunCallbacks
{
    //private PhotonVoiceNetwork voiceNetwork;
    private void Awake() {
        //this.voiceNetwork = PhotonVoiceNetwork.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
