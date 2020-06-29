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
    public Listener listener;
    public Recorder recorder;


    private void Start() {
        #if UNITY_WEBGL || UNITY_EDITOR
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

        if(recordAtStart)
        {
            recorder.StartRecord();
        }
        #endif

    }
    

    private void Update() {
        Debug.Log("Mic: " + listener.speakers.Count);
    }

    public void RefreshWebGLSpeakers()
    {
        Debug.Log("Starting refresh webgl");

        #if UNITY_WEBGL || UNITY_EDITOR
        if(listener.speakers.Count > 0)
        {
            foreach(KeyValuePair<int, Speaker> speaker in listener.Speakers)
            {
                Debug.Log("Starting with key pair: " + speaker.Value.Id);
                bool isValidSpeaker = false;

                foreach(PhotonView pv in gameManager.playerList)
                {
                    Debug.Log("Checking speaker value: " + speaker.Value.Id + " and " + pv.Owner.ActorNumber);
                    if(speaker.Value.Id == pv.Owner.ActorNumber)
                    {
                        Debug.Log("They matched");
                        speaker.Value.GetSpeaker().transform.SetParent(pv.gameObject.transform);
                        isValidSpeaker = true;
                    }                     
                }

                if(!speaker.Value.IsActive && !isValidSpeaker)
                {
                    speaker.Value.Dispose();
                }
            }
        }
        
        #endif
    }

    
}
