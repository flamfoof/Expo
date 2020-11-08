using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dissonance;
using UnityEngine.Assertions;

public class DissonanceVoiceComms : MonoBehaviour
{
    public static DissonanceVoiceComms instance;
    private DissonanceComms dissonanceRecorder;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        dissonanceRecorder = GetComponent<DissonanceComms>();

        Assert.IsNotNull(dissonanceRecorder);
    }

    public void MuteSelf(bool isMute)
    {
        dissonanceRecorder.IsMuted = !isMute;
    }

    public void MuteOther(int actorNum)
    {
        
    }
    
    public void MuteAll(bool isMute)
    {
        if(!SessionHandler.instance.CheckIfPresenter())
        {
            dissonanceRecorder.IsMuted = !isMute;
        }
    }    
}