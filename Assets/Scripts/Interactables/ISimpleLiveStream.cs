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
            ConnectStream();
            SetStatusText("");
        }
        else if (liveStream.networkEvent.Type == NetEventType.Disconnected && !liveStream.retryingConnection)
        {
            liveStream.retryingConnection = true;
            ConnectStream();
            SetStatusText("Connecting to presenter please wait...");
        }
        else if (liveStream.networkEvent.Type == NetEventType.ConnectionFailed && !liveStream.retryingConnection)
        {
            liveStream.retryingConnection = true;
            ConnectStream();
            SetStatusText("Connecting to presenter please wait...");
        }
    }

    void ConnectStream()
    {
        liveStream.StartStream();
        
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
