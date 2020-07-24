using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandCancel : CommandButton
{

    public override void Click()
    {
        IgniteGameManager.localPlayer.GetComponent<UserActions>().OpenCommandRing(false);
    }
}
