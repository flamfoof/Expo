using Photon.Voice.Unity;
using UnityEngine.Assertions;
using UnityEngine;

public class PhotonVoiceComms : MonoBehaviour
{
    public static PhotonVoiceComms instance;
    private Recorder photonRecorder;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        photonRecorder = GetComponent<Recorder>();

        Assert.IsNotNull(photonRecorder);
    }

    public void MuteSelf(bool isMute)
    {
        photonRecorder.TransmitEnabled = isMute;
    }

    public void MuteOther(int actorNum)
    {
        
    }
    
    public void MuteAll(bool isMute)
    {
        if(!SessionHandler.instance.CheckIfPresenter())
        {
            photonRecorder.TransmitEnabled = isMute;
        }
    }    
}
