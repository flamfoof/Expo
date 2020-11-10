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

    [HideInInspector]
    public PlayPauseUI playPauseUI;


    PhotonView photonView;
    VideoPlayer videoPlayer;
    YoutubePlayer ytPlayer;

    bool isVideoPlaying;

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
        {
            playPauseUI.SetVisibility(ytPlayer.autoPlayOnStart, false);
        }
    }

    public override void Perform(InputActionPhase phase)
    {
        if (phase == InputActionPhase.Started)
        {
            if (BBBAnalytics.instance)
            {
                if (videoPlayer.source == VideoSource.VideoClip)
                {
                    BBBAnalytics.instance.ClickedVideo(videoPlayer.clip.name);
                }
                else
                {
                    BBBAnalytics.instance.ClickedVideo(videoPlayer.url);
                }
            }


            photonView.RPC("InteractVideo", RpcTarget.AllBuffered);
        }
    }

    [PunRPC][UsedImplicitly]
    [ContextMenu("InteractVideo")]
    void InteractVideo()
    {
        isVideoPlaying = videoPlayer.isPlaying;

        if (lvp.isLocal)
        {
            if (isVideoPlaying)
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
            if (!ytPlayer.enabled)
            {
                videoPlayer.source = VideoSource.Url;
                ytPlayer.enabled = true;
            }

            if (isVideoPlaying)
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

    public bool PlayStatus()
    {
        return videoPlayer.isPlaying;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(isVideoPlaying);
        }
        else
        {
            this.isVideoPlaying = (bool)stream.ReceiveNext();
        }
    }
}
