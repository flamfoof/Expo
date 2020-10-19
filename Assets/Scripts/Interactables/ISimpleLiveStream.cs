using UnityEngine.InputSystem;
using UnityEngine;
using Photon.Pun;
using Byn.Unity.Examples;
using UnityEngine.Assertions;

public class ISimpleLiveStream : Interactables, IPunObservable
{
    bool checkTrigger = false;
    OneToMany liveStream;

    public GameObject [] objectsToDisable;


    private void Start()
    {
        liveStream = GetComponent<OneToMany>();

        Assert.IsNotNull(liveStream);
    }

    public override void Perform(InputActionPhase phase)
    {
        if (phase == InputActionPhase.Started)
        {
            StartEndStream();
        }
    }

    [ContextMenu("StartEndStream")]
    void StartEndStream()
    {
        checkTrigger = !checkTrigger;

        if (liveStream && !liveStream.isInitialized)
        {
            liveStream.StartStream();
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
