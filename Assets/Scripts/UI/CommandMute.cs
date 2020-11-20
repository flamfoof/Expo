using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandMute : CommandButton
{
    public bool isTransmitting = true;
    
    public override void Click()
    {
        //DissonanceVoiceComms.instance.MuteSelf(!isTransmitting);
        IgniteGameManager.IgniteInstance.mutedStateObj.SetActive(isTransmitting);
    }
}
