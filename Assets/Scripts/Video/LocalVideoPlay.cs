using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class LocalVideoPlay : MonoBehaviour
{
    public bool isLocal;
    public bool isManual;
    public bool audioOn = true;
    public YoutubePlayer yt;
    public float maxDistVolume = 12.0f;
    public float minDistVolume = 2.0f;

    //audio output is linear on default
    AudioSource audio;

    private void Start() {
        audio = GetComponent<AudioSource>();
        #if UNITY_WEBGL
        if(!audioOn)
        {
            audio.enabled = false;
            Debug.Log("Audio is off");
        }
            
        yt = GetComponent<YoutubePlayer>();

        yt.videoPlayer.audioOutputMode = VideoAudioOutputMode.Direct;
        yt.videoPlayer.SetDirectAudioVolume(0, 0.0f);

        
        #endif
    }

}
