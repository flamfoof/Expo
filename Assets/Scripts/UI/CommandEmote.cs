using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CommandEmote : CommandButton
{
    int emoteIndex = -1;
    bool isMobile = false;

    private void Start() 
    {
#if UNITY_IOS || UNITY_ANDROID
        isMobile = true;
#endif
    }

    public override void Click()
    {
        if(!isMobile)
        {
            SetEmotePlay(emoteIndex);        
        } else 
        {
            GetComponent<Button>().Select();
            ShowChildrenButton();
        }
        
    }

    public override void Hover()
    {
        GetComponent<Button>().Select();
        ShowChildrenButton();
    }

    public void SetEmotePlay(int index)
    {
        if (BBBAnalytics.instance)
        {
            BBBAnalytics.instance.EmojiUsed(GetComponentInParent<EmoteList>().emotesList[index].name);
        }


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
