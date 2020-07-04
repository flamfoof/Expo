using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_WEBGL
using Microphone = UnityEngine.Microphone;
#endif
#if !UNITY_WEBGL
using Photon.Voice;
#endif

public class SelectVoicePlatform : MonoBehaviour
{
    public GameObject webglVoice;
    public GameObject platformVoice;

    void Awake()
    {
        #if UNITY_WEBGL && !UNITY_EDITOR
        Microphone.Init();
        Microphone.QueryAudioInput();

        #endif
    }

    void Start()
    {
        #if UNITY_WEBGL
        //webglVoice.SetActive(true);
        #else

            
        #endif
        platformVoice.SetActive(true);
        
    }

    // Update is called once per frame
    void Update()
    {
        #if UNITY_WEBGL && !UNITY_EDITOR
        Microphone.Update();   
        #endif
    }
}
