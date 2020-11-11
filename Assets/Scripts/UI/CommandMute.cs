using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandMute : CommandButton
{
    public bool isTransmitting = true;
    
    public override void Click()
    {
        //IgniteGameManager.voiceManager.webRTC.GetComponent<AudioCall>().SetMuteSelf(isMuteButton);
        //isTransmitting = !isTransmitting;
        //Debug.Log("Muting: " + isTransmitting);
        DissonanceVoiceComms.instance.MuteSelf(!isTransmitting);
    }
}
