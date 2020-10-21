using UnityEngine.InputSystem;
using UnityEngine;
using Photon.Pun;
using Byn.Unity.Examples;
using UnityEngine.Assertions;
using TMPro;
using Byn.Awrtc;

public class ISimpleLiveStream : Interactables, IPunObservable
{
    bool checkTrigger = false;
    OneToMany liveStream;

    public GameObject [] objectsToDisable;

    public TextMeshProUGUI Text;

    private void Start()
    {
        liveStream = GetComponent<OneToMany>();

        Assert.IsNotNull(liveStream);
        Assert.IsNotNull(Text);
    }

    public void SetStatusText(string msg)
    {
        Text.text = msg;
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
            SetStatusText("Connecting to presenter please wait...");
        }
        else if (liveStream.networkEvent.Type == NetEventType.Disconnected)
        {
            liveStream.StartStream();
            SetStatusText("Connecting to presenter please wait...");
        }
        
        //DisableObjects(!checkTrigger);
    }

    public void DisableObjects(bool status)
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
