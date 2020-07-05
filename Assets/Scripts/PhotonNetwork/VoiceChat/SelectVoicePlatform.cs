using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if !UNITY_WEBGL
using Photon.Voice;
#endif

public class SelectVoicePlatform : MonoBehaviour
{
    public GameObject webglVoice;
    public GameObject platformVoice;

    void Start()
    {
        #if UNITY_WEBGL
        //webglVoice.SetActive(true);
        #else
        platformVoice.SetActive(true);

            
        #endif

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
