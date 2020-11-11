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

    DissonanceComms dissonanceComms;

    void Start()
    {
        if(!player.GetComponent<PhotonView>().IsMine)
        {
            //gameObject.SetActive(false);
        }

       dissonanceComms = GameObject.FindObjectOfType<DissonanceComms>();

       //_state = dissonanceComms.FindPlayer(dissonanceComms.LocalPlayerName);
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
        if(_state.IsSpeaking)
        {
            GetComponent<Image>().enabled = true;
        } else {
            GetComponent<Image>().enabled = false;
        }
        

    }
}
