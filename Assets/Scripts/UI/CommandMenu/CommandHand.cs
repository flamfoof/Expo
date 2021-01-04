using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CommandHand : CommandButton
{
    public override void Click()
    {
        UserActions player = IgniteGameManager.localPlayer.GetComponent<UserActions>();
        //print(player.handRaise.name);
        print("HAND RAISE: " + player.isHandRaised);
        if(player.isHandRaised)
        {
            player.isHandRaised = false;
            player.handRaise.SetActive(false);
        }
        else
        {
            player.isHandRaised = true;
            player.handRaise.SetActive(true);
        }
        player.HandRaiseClicked();
    }
}
