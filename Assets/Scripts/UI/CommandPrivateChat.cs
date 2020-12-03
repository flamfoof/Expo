using UnityEngine;

public class CommandPrivateChat : CommandButton
{
    private bool toggle = false;
    public override void Click()
    {
        toggle = !toggle;
        IgniteGameManager.localPlayer.GetComponent<UserActions>().OpenPrivateMessagePanel(toggle);
    }
}
