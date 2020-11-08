using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandChat : CommandButton
{

    [ContextMenu("Click")]
    public override void Click()
    {
        IgniteGameManager.localPlayer.GetComponent<UserActions>().OpenChat(true);
    }

    public void Click(bool isOpen)
    {
        IgniteGameManager.localPlayer.GetComponent<UserActions>().OpenChat(isOpen);
    }
}
