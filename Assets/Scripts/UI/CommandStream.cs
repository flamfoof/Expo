using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandStream : CommandButton
{
    public override void Click()
    {
        
    }

    public void StartStopStream(bool status)
    {
        IgniteGameManager.IgniteInstance.GetComponent<DestroyConnection>().StartEndStream(status);
    }
}
