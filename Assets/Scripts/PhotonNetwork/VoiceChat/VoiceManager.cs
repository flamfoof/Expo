using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_WEBGL
using FrostweepGames.Plugins.Native;
using FrostweepGames.WebGLPUNVoice;
#elif !UNITY_WEBGL
using Photon.Voice;
#endif
using Photon.Pun;
using Photon.Realtime;

public class VoiceManager : MonoBehaviourPunCallbacks
{
    public IgniteGameManager gameManager;
    private SelectVoicePlatform voicePlatform;
    public bool recordAtStart = true;

    //FrostSweep library
    Listener listener;
    Recorder recorder;

    #if UNITY_WEBGL
    private void Start() {
        voicePlatform = GetComponent<SelectVoicePlatform>();
        recorder = voicePlatform.webglVoice.GetComponent<Recorder>();
        listener = voicePlatform.webglVoice.GetComponent<Listener>();

        CustomMicrophone.RequestMicrophonePermission();

        if(!gameManager)
        {
            gameManager = GameObject.FindObjectOfType<IgniteGameManager>();
            if(!gameManager)
                Debug.Log("There is no game manager");
        }
        //voicePlatform.webglVoice.GetComponent<Recorder
        if(recordAtStart)
        {
            recorder.StartRecord();
        }

    }

    private void Update() {
        Debug.Log("Mic: " + listener.speakers.Count);
    }

    #endif
    
}
