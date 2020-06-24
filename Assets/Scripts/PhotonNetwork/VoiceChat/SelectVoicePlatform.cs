using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Voice;

public class SelectVoicePlatform : MonoBehaviour
{
    public GameObject webglVoice;

    void Start()
    {
        #if UNITY_WEBGL
        webglVoice.SetActive(true);
        if(GetComponent<Photon.Voice.Unity.Recorder>())
        {
            Debug.Log("Turning Photon Voice Recorder because this is a WebGL build");
            GetComponent<Photon.Voice.Unity.Recorder>().enabled = false;            
        }
        if(GetComponent<Photon.Voice.PUN.PhotonVoiceNetwork>())
        {
            Debug.Log("Turning off Photon Voice Network because this is a WebGL build");
            GetComponent<Photon.Voice.PUN.PhotonVoiceNetwork>().enabled = false;
        }
            
        #endif
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
