using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class IgniteAnalytics : MonoBehaviourPunCallbacks
{
    public virtual void ClickedStats()
    {

    }

    public virtual void ClickedStats(string url)
    {

    }
    
    public virtual void ClickedStats(int amount)
    {
        //amount is usually 1
        
    }

    public virtual void ClickedVideo(string name)
    {

    }

    public virtual void ClickedWeb(string url)
    {

    }

    public virtual void EmojiUsed(string name)
    {

    }

    public virtual void AverageSessionLength()
    {

    }

    public virtual void UpdateChatLog(string log)
    {

    }

    public virtual void ClickedStats(Interactables interacble, int amount)
    {
        
        
    }
}
