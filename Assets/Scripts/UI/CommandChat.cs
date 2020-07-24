using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandChat : CommandButton
{
    public override void Click()
    {
        IgniteGameManager.localPlayer.GetComponent<UserActions>().OpenChat(true);
    }
}
