using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CommandEmote : CommandButton
{
    int emoteIndex = -1;
    public override void Click()
    {
        SetEmotePlay(emoteIndex);        
    }

    public override void Hover()
    {
        GetComponent<Button>().Select();
        ShowChildrenButton();
    }

    public void SetEmotePlay(int index)
    {
        this.emoteIndex = index;
        IgniteGameManager.localPlayer.GetComponent<UserActions>().OpenEmoteMenu(emoteIndex);
        HideChildrenButton();
    }

    public void HideChildrenButton()
    {
        foreach(Transform childButton in transform)
        {
            if(childButton.GetComponent<CommandButton>())
            {
                childButton.gameObject.SetActive(false);
            }
        }
    }

    public void ShowChildrenButton()
    {
        foreach(Transform childButton in transform)
        {
            if(childButton.GetComponent<CommandButton>())
            {
                childButton.gameObject.SetActive(true);
            }
        }
    }

    void OnDisable()
    {
        HideChildrenButton();
    }
}
