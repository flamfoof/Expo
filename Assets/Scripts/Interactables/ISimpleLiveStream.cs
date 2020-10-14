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

    void Start()
    {
        callApp = GetComponent<CallAppUi>();

        Assert.IsNotNull(callApp);
    }
    
    public override void Perform(InputActionPhase phase)
    {
        if (phase == InputActionPhase.Started)
        {
            Debug.Log("Call Perform");
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
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        throw new System.NotImplementedException();
    }
}
