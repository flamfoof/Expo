using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Byn.Awrtc;

public class ButtonMute : MonoBehaviour
{
    AudioCall audioCall;
    public InputField iF;

    void Start()
    {
        audioCall = GameObject.FindObjectOfType<AudioCall>();
    }

    public void OnClickThing(float vol)
    {
        int val = int.Parse(iF.text);
        Debug.Log("user id is targetted at: " + val);
        ConnectionId cid = new ConnectionId();
        cid.id = (short)val;
        audioCall.ButtonMuteChannel(cid, vol);
    }

    public void VoiceReonnectTest()
    {
        int val = int.Parse(iF.text);
        audioCall.MediaReconnect(val);
    }
}
