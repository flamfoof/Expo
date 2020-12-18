using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using JetBrains.Annotations;
using Dissonance.Demo;
using Dissonance;
using Photon.Pun;

public class UIAudioBehavior : MonoBehaviour
{
    public GameObject player;
    private IDissonancePlayer _player;
    private VoicePlayerState _state;
    private float _intensity;
    public float voiceSensitivity = 0.01f;

    DissonanceComms dissonanceComms;

    void Start()
    {
       dissonanceComms = GameObject.FindObjectOfType<DissonanceComms>();

       _player = player.GetComponent<IDissonancePlayer>();

       StartCoroutine(FindPlayerState());
        
    }

    private IEnumerator FindPlayerState()
    {
        //Wait until player tracking has initialized
        while (!_player.IsTracking)
            yield return null;

        //Now ask Dissonance for the object which represents the state of this player
        //The loop is necessary in case Dissonance is still initializing this player into the network session
        while (_state == null)
        {
            _state = FindObjectOfType<DissonanceComms>().FindPlayer(_player.PlayerId);
            yield return null;
        }
    }
    

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("Speaking state: " + _state.Amplitude);
        if(_state != null)
        {
            if(_state.IsSpeaking && _state.Amplitude >= voiceSensitivity)
                GetComponent<Image>().enabled = true;
        } else {
            GetComponent<Image>().enabled = false;
        }
        

    }
}
