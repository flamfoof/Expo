using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandMuteAll : CommandButton
{
    private bool isTransmitting = false;

    private void Start()
    {
        if(!SessionHandler.instance.CheckIfPresenter())
        {
            gameObject.SetActive(false);
        }
    }

    public override void Click()
    {
        Debug.Log("muting all: " + isTransmitting);
        IgniteGameManager.localPlayer.GetComponent<UserActions>().SetMuteAll(isTransmitting);
    }
}