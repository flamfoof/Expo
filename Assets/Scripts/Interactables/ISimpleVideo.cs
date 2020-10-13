using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.Assertions;
using Photon.Voice;
using Photon.Pun;
using JetBrains.Annotations;

public class ISimpleVideo : Interactables, IPunObservable
{
    [SerializeField]
    LocalVideoPlay lvp;
    [SerializeField]
    PlayPauseUI playPauseUI;

    PhotonView photonView;
    VideoPlayer videoPlayer;
    YoutubePlayer ytPlayer;

    void Start()
    {
        photonView = GetComponent<PhotonView>();
        videoPlayer = GetComponent<VideoPlayer>();
        ytPlayer = GetComponent<YoutubePlayer>();

        lvp = GetComponent<LocalVideoPlay>();
        playPauseUI = GetComponentInChildren<PlayPauseUI>();

        Assert.IsNotNull(lvp);
        Assert.IsNotNull(playPauseUI);

        IntializeValues();
    }

    void IntializeValues()
    {
        if (lvp.isLocal)
            playPauseUI.SetVisibility(!videoPlayer.playOnAwake, false);
        else
            playPauseUI.SetVisibility(!ytPlayer.autoPlayOnStart, false);
    }

    public override void Perform(InputActionPhase phase)
    {
        if (phase == InputActionPhase.Started)
        {
            //Debug.Log("InputActionPhase.Started Isimple");
            photonView.RPC("InteractVideo", RpcTarget.AllBuffered);
        }
    }

    [PunRPC][UsedImplicitly]
    void InteractVideo()
    {
        Debug.Log("InteractVideo RPC received");
        if (lvp.isLocal)
        {
            if (videoPlayer.isPlaying)
            {
                playPauseUI.SetVisibility(true, false);
                videoPlayer.Pause();
            }
            else
            {
                playPauseUI.SetVisibility(true, true);
                videoPlayer.Play();
            }
        }
        else
        {
            if (ytPlayer.isSyncing)
            {
                playPauseUI.SetVisibility(true, false);
                videoPlayer.Pause();
            }
            else
            {
                playPauseUI.SetVisibility(true, true);
                videoPlayer.Play();
            }
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        throw new System.NotImplementedException();
    }
}
