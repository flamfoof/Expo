using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandMuteAll : CommandButton
{
    public bool isTransmitting = false;

    private void OnEnable()
    {
        bool admin = SessionHandler.instance.CheckIfPresenter() || SessionHandler.instance.CheckIfStaff() ? true : false;
        if(!admin)
        {
            Debug.Log("Not a " + SessionHandler.instance.CheckIfPresenter() + " or a " + SessionHandler.instance.CheckIfStaff());
            gameObject.SetActive(false);
        }
    }

    public override void Click()
    {
        Debug.Log("muting all: " + isTransmitting);
        IgniteGameManager.localPlayer.GetComponent<UserActions>().SetMuteAll(!isTransmitting);
    }
}