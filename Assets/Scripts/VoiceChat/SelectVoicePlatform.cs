using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if !UNITY_WEBGL
//using Photon.Voice;
#endif

public class SelectVoicePlatform : MonoBehaviour
{
    public GameObject webglVoice;
    public GameObject webRTCVoice;
    public GameObject platformVoice;

    void Awake()
    {
        //for now we are moving towards WebRTC for crossplatform communication
        //webRTCVoice.SetActive(true);
        
        
        #if UNITY_WEBGL
        webRTCVoice.SetActive(true);
        platformVoice.SetActive(false);
        #else
        platformVoice.SetActive(true);
        webRTCVoice.SetActive(false);
            
        #endif

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
