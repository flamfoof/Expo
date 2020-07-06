using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalVideoPlay : MonoBehaviour
{
    public bool isLocal;
    public bool isManual;
    public bool isAudio = true;

    AudioSource audio;

    private void Start() {
        audio = GetComponent<AudioSource>();
        #if UNITY_WEBGL
        if(!isAudio)
            audio.enabled = false;
            Debug.Log("Audio is off");
        #endif
    }


}
