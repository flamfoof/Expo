using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_WEBGL
#elif !UNITY_WEBGL
//using Photon.Voice;
#endif
using Photon.Pun;
using Photon.Realtime;

public class CommunicationManager : MonoBehaviourPunCallbacks
{
    public IgniteGameManager gameManager;
    private SelectVoicePlatform voicePlatform;
    public AudioCall webRTC;
    public GameObject sendMessage;
    public GameObject receiveText;
    public bool recordAtStart = true;
    public float minAudioDistance = 0.0f;
    public float maxAudioDistance = 15.0f;

    //FrostSweep library



    private void Awake() {
        #if UNITY_WEBGL
        voicePlatform = GetComponent<SelectVoicePlatform>();
        //recorder = voicePlatform.webglVoice.GetComponent<Recorder>();
        //listener = voicePlatform.webglVoice.GetComponent<Listener>();

        //CustomMicrophone.RequestMicrophonePermission();
        /*
        if(!gameManager)
        {
            gameManager = GameObject.FindObjectOfType<IgniteGameManager>();
            if(!gameManager)
                Debug.Log("There is no game manager");
        }*/

        //listener.SpeakersUpdatedEvent += RefreshWebGLSpeakers;

        if(recordAtStart)
        {
            //recorder.StartRecord();
        }
        #endif
        
    }

    /*
    private void Start() {
        
        if(recordAtStart)
        {
            Debug.Log("Started Recording");

            recorder.StartRecord();
            
        }
        
    }*/
    

    public void RefreshWebGLSpeakers()
    {
        /*
        //Debug.Log("Starting refresh webgl" + listener.name);
        //Debug.Log("Starting refresh webgl local p#: " + PhotonNetwork.LocalPlayer.ActorNumber);
        if(!CustomMicrophone.IsRecording(CustomMicrophone.devices[0]))
        {
            Debug.Log("Refresh: Not recording, starting to");
            Debug.Log("Mic permission: " + CustomMicrophone.HasMicrophonePermission());
            Debug.Log("Mic connected?: " + CustomMicrophone.HasConnectedMicrophoneDevices());
            recorder.StartRecord();
            recorder.CheckIfPlayerIsRecording(IgniteGameManager.localPlayer);
            Invoke("RefreshWebGLSpeakers", 1.0f);
            return;
        }

        #if UNITY_WEBGL
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
                        speaker.Value.GetSpeaker().GetComponent<AudioSource>().spatialize = true;
                        speaker.Value.GetSpeaker().GetComponent<AudioSource>().spatialBlend = 1.0f;
                        speaker.Value.GetSpeaker().GetComponent<AudioSource>().minDistance = minAudioDistance;
                        speaker.Value.GetSpeaker().GetComponent<AudioSource>().maxDistance = maxAudioDistance;
                        speaker.Value.GetSpeaker().GetComponent<AudioSource>().rolloffMode = AudioRolloffMode.Linear;
                        
                        isValidSpeaker = true;
                    }                     
                }

                if(!speaker.Value.IsActive && !isValidSpeaker)
                {
                    Debug.Log("Disposed");
                    speaker.Value.Dispose();
                }
            }
        } else if (listener.speakers.Count == 0)
        {
            CustomMicrophone.RequestMicrophonePermission();
            recorder.StartRecord();
        }
        
        if(!CustomMicrophone.HasConnectedMicrophoneDevices())
        {
            Debug.Log("No microphones were connected");
        }
        #endif
        */
    }

    
}
