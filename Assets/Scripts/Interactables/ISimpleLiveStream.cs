using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.Assertions;
using Photon.Voice;
using Photon.Pun;
using JetBrains.Annotations;

public class ISimpleLiveStream : Interactables, IPunObservable
{
    CallAppUi callApp;

    bool checkTrigger = false;

    public GameObject [] objectsToDisable;

    void Start()
    {
        callApp = GetComponent<CallAppUi>();

        Assert.IsNotNull(callApp);
    }
    
    public override void Perform(InputActionPhase phase)
    {
        if (phase == InputActionPhase.Started)
        {
            StartEndCall();
        }
    }

    [ContextMenu("StartEndCall")]
    void StartEndCall()
    {
        checkTrigger = !checkTrigger;

        if (checkTrigger)
        {
            Debug.Log("Call started");
            callApp.SetupCallApp();
        }
        else
        {
            Debug.Log("Call ended");
            callApp.ShutdownButtonPressed();
        }

        DisableObjects(!checkTrigger);
    }

    void DisableObjects(bool status)
    {
        if (objectsToDisable.Length == 0)
            return;

        for (int i = 0; i < objectsToDisable.Length; i++)
        {
            if (objectsToDisable[i] != null)
            {
                objectsToDisable[i].SetActive(status);
            }
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        throw new System.NotImplementedException();
    }
}
