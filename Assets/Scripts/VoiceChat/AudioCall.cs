using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Byn.Awrtc;
using Byn.Awrtc.Unity;
using Byn.Unity.Examples;
using UnityEngine.UI;
 
public class AudioCall : MonoBehaviourPunCallbacks
{
    public string voiceID = "";
    public AudioCallUI audioCallUI;

    public string uSignalingUrl = "ws://rtc-ignite-voice.herokuapp.com/conferenceapp";

    public string uSecureSignalingUrl = "wss://rtc-ignite-voice.herokuapp.com/conferenceapp";


    public bool uForceSecureSignaling = false;

    public string uIceServer = "turn:numb.viagenie.ca";

    //
    public string uIceServerUser = "webrtc@live.com";
    public string uIceServerPassword = "muazkh";
    public string uIceServer2 = "stun:stun.l.google.com:19302";
    public string uIceServer3 = "stun:stun.stunprotocol.org:3478";
    public string uIceServer4 = "stun:stunserver.org:3478";
    public string uIceServer5 = "stun:stun.yesss.at:3478";
    public string uIceServer6 = "stun:stun1.l.google.com:19302";
    public string uIceServer7 = "stun:stun.voiparound.com";

    public const int MAX_CODE_LENGTH = 256;

    protected ICall iCall;

    private IMediaNetwork mediaNetwork = null;

    public UserPermissionCommunication userAllowPermissions;

    public bool audioOn = true;
    public bool userAllowAudio = true;
    public bool videoOn = false;
    public bool userAllowVideo = false;

    protected MediaConfig mMediaConfig;
    private bool isICall = false;
    private bool isMediaNetwork = true;
    private bool startedVoiceServer = false;

    public InputField messageField;

    public ChatText textOutput;

    public GameObject receiveTxt;
    public bool lockChatVisibility = false;

    public Scrollbar scrollbar;

    protected bool iCallActive = false;
    private string localAddress = null;
    protected string targetAddress = null;
    private static string sAddress = null;
    protected MediaConfig mediaConfigUse;
    protected ConnectionId remoteUserId = ConnectionId.INVALID;

    private List<ConnectionId> connectionIdList = new List<ConnectionId>();
    protected bool autoRejoin = true;
    protected IEnumerator autoRejoinCoroutine = null;
    protected float rejoinTime = 0.2f;
    private float mediaNetworkConnectionTime = 2.0f;

    protected bool localFrameEvents = true;

    public bool isConference = true;


    public GameObject[] videoImage;
    public List<int> idWithVideos = new List<int>();
    public List<ConnectionId> idConnectionVid = new List<ConnectionId>();
    public List<int> connectedVoiceID = new List<int>();
    public int videoCount = 0;
    public ConnectionId idSingleVideo;
    public bool firstJoinedVid = false;

    public Texture2D noVidTexture;

    private bool forceVideoOff = true;

    private class VideoData
    {
        public GameObject uiObject;
        public Texture2D texture;
        public RawImage image;

    }
 
   private Dictionary<ConnectionId, VideoData> mVideoUiElements = new Dictionary<ConnectionId, VideoData>();
  
    private void Awake() {
        userAllowPermissions = UserPermissionCommunication.instance;
        mMediaConfig = CreateMediaConfig();
        mediaConfigUse = mMediaConfig;    
        if(mVideoUiElements.Count > 0)
            SetupVideoUiEmpty();      

        if(isICall)
        {
            //Disabling ICall Events
            InvokeRepeating("GetRoomID", 1.0f, 1.0f);

            //in case we have any UI we want to add in.
            if(GetComponent<AudioCallUI>())
            {
                audioCallUI = GetComponent<AudioCallUI>();
            }
            StartCoroutine(ExampleGlobals.RequestPermissions(true, false));
            UnityCallFactory.EnsureInit(OnCallFactoryReadyICall, OnCallFactoryFailedICall);
        }
    }
 
    void Start()
    {
        scrollbar.value = 0;

        mMediaConfig = CreateMediaConfig();
        mediaConfigUse = mMediaConfig;  

        userAllowPermissions = UserPermissionCommunication.instance;


        //StartCoroutine(ExampleGlobals.RequestPermissions(audioOn, videoOn));
        SetupVideoUi(ConnectionId.INVALID);
        UnityCallFactory.EnsureInit(OnCallFactoryReadyMediaNetwork, OnCallFactoryFailedMediaNetwork);
        
        //Disabling ICall Events
        InvokeRepeating("GetRoomID", 1.0f, 1.0f);

        //in case we have any UI we want to add in.
        if(GetComponent<AudioCallUI>())
        {
            audioCallUI = GetComponent<AudioCallUI>();
        }
        //StartCoroutine(ExampleGlobals.RequestPermissions(true, true));
        //UnityCallFactory.EnsureInit(OnCallFactoryReady, OnCallFactoryFailed);
    }
 
    void Update()
    {
        if(isMediaNetwork)
        {
            if (mediaNetwork == null)
                return;

            mediaNetwork.Update();

            //This is the event handler via polling.
            //This needs to be called or the memory will fill up with unhanded events!
            NetworkEvent evt;
            while (mediaNetwork != null && mediaNetwork.Dequeue(out evt))
            {
                HandleNetworkEvent(evt);
            }
            //polls for video updates
            HandleMediaEvents();

            //Flush will resync changes done in unity to the native implementation
            //(and possibly drop events that aren't handled in the future)
            if (mediaNetwork != null)
                mediaNetwork.Flush();
        }

        if (iCall != null)
        {
            iCall.Update();
        }
        
    }
 
    #region ICall

    protected virtual void OnCallFactoryReadyICall()
    {
        //set to warning for regular use
        UnityCallFactory.Instance.RequestLogLevel(UnityCallFactory.LogLevel.Info);
    }

    protected virtual void OnCallFactoryFailedICall(string error)
    {
        string fullErrorMsg = typeof(CallApp).Name + " can't start. The " + typeof(UnityCallFactory).Name + " failed to initialize with following error: " + error;
        Debug.LogError(fullErrorMsg);
    }

    void GetRoomID()
    {
        Debug.Log("Finding Voice Room ID...");
        
        if(voiceID == "")
        {                                     
            ConnectToRoom();
        } else {
            Debug.Log("Failed to find Voice ID");
        }
    }

    public void ConnectToRoom()
    {
        voiceID = "Voice_" + PhotonNetwork.CurrentRoom.Name;
        Debug.Log(voiceID);        
        Debug.Log("Sucessfully found a room");
        CancelInvoke("GetRoomID");
        SetupCall();
        Join(voiceID);
    }

    public virtual MediaConfig CreateMediaConfig()
    {
        MediaConfig mediaConfig = new MediaConfig();
        //testing echo cancellation (native only)
        bool useEchoCancellation = false;
        if(useEchoCancellation)
        {
            #if (!UNITY_WEBGL && !UNITY_WSA)
            var nativeConfig = new Byn.Awrtc.Native.NativeMediaConfig();
            nativeConfig.AudioOptions.echo_cancellation = true;
            nativeConfig.AudioOptions.extended_filter_aec = true;
            nativeConfig.AudioOptions.delay_agnostic_aec = true;

            mediaConfig = nativeConfig;
            #endif
        }

        Debug.Log("Example vid dev is: " + UnityCallFactory.Instance.GetDefaultVideoDevice());
        if(userAllowPermissions)
        {
            Debug.Log("Video perm is defined: " + userAllowPermissions.allowVideo);

            if(UnityCallFactory.Instance.GetDefaultVideoDevice() != "" && userAllowPermissions.allowVideo)
            {
                Debug.Log("There exists a video thing and user allowed it.");
                videoOn = true;    
            }
        }
        
        

        mediaConfig.Audio = audioOn;
        Debug.Log("Audio Perm is: " + mediaConfig.Audio);
        if(forceVideoOff)
        {
            mediaConfig.Video = false;
        } else
        {
            mediaConfig.Video = videoOn;
        }
        

        Debug.Log("Video Perm is: " + mediaConfig.Video);

        mediaConfig.VideoDeviceName = UnityCallFactory.Instance.GetDefaultVideoDevice();

        //This format is the only reliable format that works on all
        //platforms currently.
        mediaConfig.Format = FramePixelFormat.ABGR;

        //original lines
        mediaConfig.MinWidth = 160;
        mediaConfig.MinHeight = 120;
        mediaConfig.MaxWidth = 1920;
        mediaConfig.MaxHeight = 1080;

        //will be overwritten by UI in normal use
        mediaConfig.IdealWidth = 160;
        mediaConfig.IdealHeight = 120;
        mediaConfig.IdealFrameRate = 30;
        
        /*
        mediaConfig.MinWidth = 0;
        mediaConfig.MinHeight = 0;
        mediaConfig.MaxWidth = 0;
        mediaConfig.MaxHeight = 0;
        mediaConfig.IdealWidth = 0;
        mediaConfig.IdealHeight = 0;
        mediaConfig.IdealFrameRate = 30;*/

        return mediaConfig;
    }

    public void SetupCall()
    {
        Debug.Log("Setting up ...");

        NetworkConfig netConfig = CreateNetworkConfig();
        //this is what we need for multiple audio connections
        netConfig.IsConference = true;

        Debug.Log("Creating call using NetworkConfig:" + netConfig);
        iCall = CreateCall(netConfig);
        if (iCall == null)
        {
            Debug.Log("Failed to create the call");
            return;
        }


        if(videoOn)
        {
            iCall.LocalFrameEvents = localFrameEvents;
        } else
        {
            iCall.LocalFrameEvents = false;
        }
        
        string[] devices = UnityCallFactory.Instance.GetVideoDevices();
        if (devices == null || devices.Length == 0)
        {
            Debug.Log("no device found or no device information available");
        }
        else
        {
            foreach (string s in devices)
                Debug.Log("device found: " + s + " IsFrontFacing: " + UnityCallFactory.Instance.IsFrontFacing(s));
        }
        Debug.Log("Call created!");
        iCall.CallEvent += Call_CallEvent;

        Debug.Log("Creating deep clone");


        //make a deep clone to avoid confusion if settings are changed
        //at runtime.
        mediaConfigUse = mMediaConfig.DeepClone();
        Debug.Log("vid/aud perm: " + mediaConfigUse.Video + " " + mediaConfigUse.Audio);

        //try to pick a good default video device if the user wants to send video but
        //didn't bother to pick a specific device
        if (mediaConfigUse.Video && string.IsNullOrEmpty(mediaConfigUse.VideoDeviceName))
        {
            mediaConfigUse.VideoDeviceName = UnityCallFactory.Instance.GetDefaultVideoDevice();
        }

        Debug.Log("Configure call using MediaConfig: " + mediaConfigUse);
        iCall.Configure(mediaConfigUse);
        voiceID = CheckRoomLength(voiceID);
        Debug.Log("Trying to listen on address " + voiceID);
    }

    protected virtual ICall CreateCall(NetworkConfig netConfig){
        //setup the server
        return UnityCallFactory.Instance.Create(netConfig);
    }

    public NetworkConfig CreateNetworkConfig()
    {
        NetworkConfig netConfig = new NetworkConfig();
        
        if (string.IsNullOrEmpty(uIceServer) == false)
        {
            netConfig.IceServers.Add(new IceServer(uIceServer, uIceServerUser, uIceServerPassword));
            Debug.Log("Connected to RTC: " + uIceServer);
        }
        if (string.IsNullOrEmpty(uIceServer2) == false)
        {
            netConfig.IceServers.Add(new IceServer(uIceServer2));
            Debug.Log("Connected to RTC: " + uIceServer2);
        } else if (string.IsNullOrEmpty(uIceServer3) == false)
        {
            netConfig.IceServers.Add(new IceServer(uIceServer3));
            Debug.Log("Connected to RTC: " + uIceServer3);
        } else if (string.IsNullOrEmpty(uIceServer4) == false)
        {
            netConfig.IceServers.Add(new IceServer(uIceServer4));
            Debug.Log("Connected to RTC: " + uIceServer4);
        } else if (string.IsNullOrEmpty(uIceServer5) == false)
        {
            netConfig.IceServers.Add(new IceServer(uIceServer5));
            Debug.Log("Connected to RTC: " + uIceServer5);
        } else if (string.IsNullOrEmpty(uIceServer6) == false)
        {
            netConfig.IceServers.Add(new IceServer(uIceServer6));
            Debug.Log("Connected to RTC: " + uIceServer6);
        } else if (string.IsNullOrEmpty(uIceServer7) == false)
        {
            netConfig.IceServers.Add(new IceServer(uIceServer7));
            Debug.Log("Connected to RTC: " + uIceServer7);
        }           
        
            

        if (Application.platform == RuntimePlatform.WebGLPlayer || uForceSecureSignaling)
        {
            netConfig.SignalingUrl = uSecureSignalingUrl;
        }
        else
        {
            netConfig.SignalingUrl = uSignalingUrl;
        }

        if (netConfig.SignalingUrl == "")
        {
            throw new InvalidOperationException("set signaling url is empty");
        }
        return netConfig;
    }

    protected virtual void Call_CallEvent(object sender, CallEventArgs e)
    {
        switch (e.Type)
        {
            case CallEventType.CallAccepted:
                //Outgoing call was successful or an incoming call arrived

                remoteUserId = ((CallAcceptedEventArgs)e).ConnectionId;
                OnNewCall(e as CallAcceptedEventArgs);
                break;
            case CallEventType.CallEnded:
                //Call was ended / one of the users hung up -> reset the app
                Debug.Log("Call ended");
                InternalResetCall();
                OnCallEnded(e as CallEndedEventArgs);
                break;
            case CallEventType.ListeningFailed:
                /*
                if(!isConference)
                {
                    iCall.Call(targetAddress);
                } else {
                    iCall.Listen(targetAddress);
                }*/
                iCall.Call(targetAddress);
                    
                
                break;

            case CallEventType.ConnectionFailed:
                {
                    ErrorEventArgs args = e as ErrorEventArgs;
                    Debug.Log("Connection failed error: " + args.Info);
                    InternalResetCall();
                }
                break;
            case CallEventType.ConfigurationFailed:
                {
                    ErrorEventArgs args = e as ErrorEventArgs;
                    Debug.Log("Configuration failed error: " + args.Info);
                    InternalResetCall();
                }
                break;

            case CallEventType.FrameUpdate:
                {                   
                    FrameUpdateEventArgs frameargs = e as FrameUpdateEventArgs;
                    UpdateFrame(frameargs.ConnectionId, frameargs.Frame);
                    break;
                }

            case CallEventType.Message:
                {
                    //text message received
                    MessageEventArgs args = e as MessageEventArgs;
                    Append(args.Content);
                    //receiveTxt.text = args.Content;
                    Debug.Log(args.Content);
                    break;
                }
            case CallEventType.WaitForIncomingCall:
                {
                    //the chat app will wait for another app to connect via the same string
                    WaitForIncomingCallEventArgs args = e as WaitForIncomingCallEventArgs;
                    Debug.Log("Waiting for incoming call address: " + args.Address);
                    break;
                }
        }

    }

    private void OnNewCall(CallAcceptedEventArgs args)
    {       
        SetupVideoUi(args.ConnectionId);
    }

    private void OnCallEnded(CallEndedEventArgs args)
    {
        VideoData data;
        if (mVideoUiElements.TryGetValue(args.ConnectionId, out data))
        {
            mVideoUiElements.Remove(args.ConnectionId);
        }
    }

    private void SetupVideoUi(ConnectionId id)
    {
        //create texture + ui element
        VideoData vd = new VideoData();
        if(videoCount >= 3)
        {
            videoCount = 0;
        }
        vd.uiObject = videoImage[0];
        videoCount++;
        vd.image = vd.uiObject.GetComponent<RawImage>();
        vd.image.texture = noVidTexture;
        Debug.Log("Video element set: " + id.id);
        mVideoUiElements[id] = vd;
    }

    private void SetupVideoUiEmpty()
    {
        for(int i = 0; i < videoImage.Length; i++)
        {
            videoImage[i].GetComponent<RawImage>().texture = noVidTexture;
        }
    }

    private void UpdateFrame(ConnectionId id, IFrame frame)
    {
        //Debug.Log("Updated consideration id: " + id.id + " does it have it?: " + mVideoUiElements.ContainsKey(id));

        if (mVideoUiElements.ContainsKey(id))
        {         
            /* 
            if(idConnectionVid.Count < videoImage.Length && !idConnectionVid.Contains(id))
            {
                idConnectionVid.Add(id);
                mVideoUiElements[id].uiObject = videoImage[idConnectionVid.Count];
                mVideoUiElements[id].image = videoImage[idConnectionVid.Count].GetComponent<RawImage>();               
            }

            //for multiple videos
            VideoData videoData = mVideoUiElements[id];
            UpdateTexture(ref videoData.texture, frame);
            videoData.image.texture = videoData.texture;
            Debug.Log("Rendering: " + id);
            */
            
            
            //for single video, later on I can identify which player to use           
            if(!firstJoinedVid)
            {
                idSingleVideo = id;
                firstJoinedVid = true;
                //Debug.Log("Set video id: " + idSingleVideo);
                //gets the first frame to render video
                mVideoUiElements[id].uiObject = videoImage[0];
                mVideoUiElements[id].image = videoImage[0].GetComponent<RawImage>();
                mVideoUiElements[id].image.texture = noVidTexture;
            }

            VideoData videoData = mVideoUiElements[id];
            UpdateTexture(ref videoData.texture, frame);
            videoData.image.texture = videoData.texture;
            //Debug.Log("Rendering: " + id);
        
            
            
        }
    }

    private void UpdateTexture(ref Texture2D tex, IFrame frame)
        {
            //texture exists but has the wrong height /width? -> destroy it and set the value to null
            if (tex != null && (tex.width != frame.Width || tex.height != frame.Height))
            {
                Texture2D.Destroy(tex);
                tex = null;
            }
            //no texture? create a new one first
            if (tex == null)
            {
                tex = new Texture2D(frame.Width, frame.Height, TextureFormat.RGBA32, false);
                tex.wrapMode = TextureWrapMode.Clamp;
            }
            //Debug.Log("Texturing");
            ///copy image data into the texture and apply
            tex.LoadRawTextureData(frame.Buffer);
            tex.Apply();
        }


    public void InternalResetCall()
    {
        CleanupCall();
        if (autoRejoin)
        {
            TriggerRejoinTimer();
        }
    }

    public void LeaveCall()
    {
        CleanupCall();
    }

    protected virtual void CleanupCall()
    {
        if (iCall != null)
        {
            firstJoinedVid = false;
            iCallActive = false;
            remoteUserId = ConnectionId.INVALID;
            Debug.Log("Destroying call!");
            iCall.CallEvent -= Call_CallEvent;
            mVideoUiElements.Clear();
            if(mVideoUiElements.Count > 0)
                mVideoUiElements.Clear();
            if(idWithVideos.Count > 0)
                idWithVideos.Clear();
            if(idConnectionVid.Count > 0)
                idConnectionVid.Clear();
            iCall.Dispose();
            iCall = null;
            //call the garbage collector. This isn't needed but helps discovering
            //memory bugs early on.
            Debug.Log("Triggering garbage collection");
            GC.Collect();
            GC.WaitForPendingFinalizers();
            Debug.Log("Call destroyed");
        }
    }

    private void TriggerRejoinTimer()
    {
        Debug.Log("Restarting in " + rejoinTime + " seconds!");
        autoRejoinCoroutine = CoroutineRejoin();
        StartCoroutine(autoRejoinCoroutine);
    }

    private IEnumerator CoroutineRejoin()
    {
        yield return new WaitForSecondsRealtime(rejoinTime);
        SetupCall();
        InternalJoin();
    }

    private void InternalJoin()
    {
        if (iCallActive)
        {
            Debug.LogError("Join call failed. Call is already/still active");
            return;
        }
        Debug.Log("Try listing on address: " + targetAddress);
        if(IgniteGameManager.IgniteInstance.gameTesting)
            SendMsg(PhotonNetwork.NickName + ": " + "Try listing on address: " + targetAddress);
        iCallActive = true;
        this.iCall.Listen(targetAddress);
    }

    public virtual void Join(string address)
    {
        if (address.Length > MAX_CODE_LENGTH)
            throw new ArgumentException("Address can't be longer than " + MAX_CODE_LENGTH);
        targetAddress = address;
        InternalJoin();
    }


    public string CheckRoomLength(string roomName)
    {
        if(roomName.Length > MAX_CODE_LENGTH)
        {
            return roomName.Substring(0, CallApp.MAX_CODE_LENGTH);
        } else {
            return roomName;
        }
    }
    #endregion ICall

    #region MediaNetwork
    protected virtual void OnCallFactoryReadyMediaNetwork()
    {
        UnityCallFactory.Instance.RequestLogLevel(UnityCallFactory.LogLevel.Info);
        if(PhotonNetwork.LocalPlayer.ActorNumber != -1)
        {
            //PhotonNetwork.PlayerList;
            StartCoroutine(InitWebRTC(PhotonNetwork.LocalPlayer.ActorNumber));
        }
            //StartCoroutine(InitWebRTC(PhotonNetwork.LocalPlayer.ActorNumber));
            
    }

    public void MediaReconnect(int val)
    {
        StartCoroutine(InitWebRTC(val));
    }

    protected virtual void OnCallFactoryFailedMediaNetwork(string error)
    {
        string fullErrorMsg = typeof(CallApp).Name + " can't start. The " + typeof(UnityCallFactory).Name + " failed to initialize with following error: " + error;
        Debug.LogError(fullErrorMsg);
    }

    /// <summary>
    /// Init process. Sets configuration and triggers the connection process
    /// </summary>
    /// <returns>
    /// Returns IEnumerator so unity treats it as a Coroutine
    /// </returns>
    private IEnumerator InitWebRTC(int playerActorID)
    {        
        if (sAddress == null)
        {
            sAddress = "Voice_" + PhotonNetwork.CurrentRoom.Name + "_User_" + playerActorID;
            Debug.Log("PLAYER IS: " + sAddress);
            localAddress = "Voice_" + PhotonNetwork.CurrentRoom.Name + "_User_" + PhotonNetwork.LocalPlayer.ActorNumber;
        }

        if (UnityCallFactory.Instance == null)
        {
            Debug.LogError("No access to webrtc. ");
        } else if (connectedVoiceID.Contains(playerActorID))
        {
            Debug.Log("This connection is already being used");
            sAddress = null;
        } else
        {
            UnityCallFactory.Instance.RequestLogLevel(UnityCallFactory.LogLevel.Info);
            //Factory works. Prepare Peers
            NetworkConfig netConfig = new NetworkConfig();

            if (string.IsNullOrEmpty(uIceServer) == false)
            {
                netConfig.IceServers.Add(new IceServer(uIceServer, uIceServerUser, uIceServerPassword));
                //Debug.Log("Connected to RTC: " + uIceServer);
            }
            if (string.IsNullOrEmpty(uIceServer2) == false)
            {
                netConfig.IceServers.Add(new IceServer(uIceServer2));
                //Debug.Log("Connected to RTC: " + uIceServer2);
            } /*
            if (string.IsNullOrEmpty(uIceServer3) == false)
            {
                netConfig.IceServers.Add(new IceServer(uIceServer3));
                //Debug.Log("Connected to RTC: " + uIceServer3);
            } 
            if (string.IsNullOrEmpty(uIceServer4) == false)
            {
                netConfig.IceServers.Add(new IceServer(uIceServer4));
                //Debug.Log("Connected to RTC: " + uIceServer4);
            } 
            if (string.IsNullOrEmpty(uIceServer5) == false)
            {
                netConfig.IceServers.Add(new IceServer(uIceServer5));
                //Debug.Log("Connected to RTC: " + uIceServer5);
            } 
            if (string.IsNullOrEmpty(uIceServer6) == false)
            {
                netConfig.IceServers.Add(new IceServer(uIceServer6));
                //Debug.Log("Connected to RTC: " + uIceServer6);
            } 
            if (string.IsNullOrEmpty(uIceServer7) == false)
            {
                netConfig.IceServers.Add(new IceServer(uIceServer7));
                //Debug.Log("Connected to RTC: " + uIceServer7);
            }     */

            if (Application.platform == RuntimePlatform.WebGLPlayer || uForceSecureSignaling)
            {
                netConfig.SignalingUrl = uSecureSignalingUrl;
            }
            else
            {
                netConfig.SignalingUrl = uSignalingUrl;
            }

            mediaNetwork = UnityCallFactory.Instance.CreateMediaNetwork(netConfig);

            //keep track of multiple local instances for testing.
            /*
            mIndex = sInstances;
            sInstances++;
            Debug.Log("Instance " + mIndex + " created.");*/

            if (videoOn && audioOn)
            {
                //sender will broadcast audio and video
                mMediaConfig.Audio = audioOn;
                mMediaConfig.Video = videoOn;

                if(!startedVoiceServer)
                {
                    Debug.Log("Accepting incoming connections on " + localAddress);
                    mediaNetwork.Configure(mMediaConfig);
                    mediaNetwork.StartServer(localAddress);
                    mediaNetwork.StartServer(localAddress);
                    startedVoiceServer = true;
                }                
            } else if(audioOn && !videoOn)
            {
                mMediaConfig.Audio = audioOn;
                mMediaConfig.Video = videoOn;

                if(!startedVoiceServer)
                {
                    Debug.Log("Accepting incoming connections on " + localAddress);
                    mediaNetwork.Configure(mMediaConfig);
                    mediaNetwork.StartServer(localAddress);
                    startedVoiceServer = true;
                }    
            } else if(!audioOn && videoOn)
            {
                mMediaConfig.Audio = audioOn;
                mMediaConfig.Video = videoOn;

                if(!startedVoiceServer)
                {
                    Debug.Log("Accepting incoming connections on " + localAddress);
                    mediaNetwork.Configure(mMediaConfig);
                    mediaNetwork.StartServer(localAddress);
                    startedVoiceServer = true;
                }    
            } else
            {
                //this one will just receive (but could also send if needed)
                mMediaConfig.Audio = false;
                mMediaConfig.Video = false;
                if(!startedVoiceServer)
                {
                    mediaNetwork.Configure(mMediaConfig);
                    startedVoiceServer = true;
                }    
            }
            Debug.Log("Very Trying to connect to " + sAddress);
            //wait a while before trying to connect othe sender
            //so it has time to register at the signaling server
            yield return new WaitForSeconds(mediaNetworkConnectionTime);
            if(!connectedVoiceID.Contains(playerActorID))
            {
                if (videoOn == false)
                {
                    Debug.Log("Trying to connect to " + sAddress);
                    mediaNetwork.Connect(sAddress);
                    
                    connectedVoiceID.Add(playerActorID);
                    Debug.Log("Is it succes?");
                    sAddress = null;
                }
            } else 
            {
                Debug.Log("The address has already been connected to: " + sAddress);
                sAddress = null;
            }            
        }
    }

    private void MediaNetworkDispose()
    {
        if(mediaNetwork != null)
        {
            mediaNetwork.Dispose();
            mediaNetwork = null;
            startedVoiceServer = false;
        }        
    }

    private void OnDestroy()
    {
        //Destroy the network
        if (mediaNetwork != null)
        {
            mediaNetwork.Dispose();
            mediaNetwork = null;
            Debug.Log("Instance " + PhotonNetwork.LocalPlayer.ActorNumber + " destroyed.");
        }
    }

    /// <summary>
    /// Handler polls the media network to check for new video frames.
    /// 
    /// </summary>
    protected virtual void HandleMediaEvents()
    {
        //just for debugging
        bool handleLocalFrames = true;
        bool handleRemoteFrames = true;

        if (mediaNetwork != null && handleLocalFrames)
        {
            IFrame localFrame = mediaNetwork.TryGetFrame(ConnectionId.INVALID);
            if (localFrame != null)
            {

            }
        }
        if (mediaNetwork != null && handleRemoteFrames)
        {
            //so far the loop shouldn't be needed. we only expect one
            foreach (var id in connectionIdList)
            {
                if (mediaNetwork != null)
                {
                    IFrame remoteFrame = mediaNetwork.TryGetFrame(id);
                    if (remoteFrame != null)
                    {

                    }
                }
            }
        }
    }


    /// <summary>
    /// Method is called to handle the network events triggered by the internal media network and 
    /// trigger related event handlers for the call object.
    /// </summary>
    /// <param name="evt"></param>
    protected virtual void HandleNetworkEvent(NetworkEvent evt)
    {
        switch (evt.Type)
        {
            case NetEventType.NewConnection:

                connectionIdList.Add(evt.ConnectionId);
                Log("New connection id " + evt.ConnectionId);

                break;
            case NetEventType.ConnectionFailed:
                //call failed
                Log("Outgoing connection failed");

                break;
            case NetEventType.Disconnected:

                if (connectionIdList.Contains(evt.ConnectionId))
                {
                    connectionIdList.Remove(evt.ConnectionId);

                    Log("Connection disconnected");
                }
                break;
            case NetEventType.ServerInitialized:
                //incoming calls possible
                Log("Server ready for incoming connections. Address: " + evt.Info);
                break;
            case NetEventType.ServerInitFailed:
                Log("Server init failed");
                break;
            case NetEventType.ServerClosed:
                Log("Server stopped");
                break;
        }
    }
    #endregion MediaNetwork


    /// <summary>
    /// Log method to help seeing what each of the different apps does.
    /// </summary>
    /// <param name="txt"></param>
    private void Log(string txt)
    {
        Debug.Log("Instance " + PhotonNetwork.LocalPlayer.ActorNumber + ": " + txt);        
    }

    /// <summary>
    /// This is called if the send button
    /// </summary>
    public void SendButtonPressed()
    {
        //get the message written into the text field
        string msg = PhotonNetwork.NickName + ": " + messageField.text;
        char slash = (char)'/';
        if(messageField.text[0] == slash)
        {
            Debug.Log("Chat command activated");
            ChatCommand(messageField.text);
            return;
        }

        SendMsg(msg);
    }

    public void ChatCommand(string msg)
    {
        switch(msg)
        {
            case "/video":
                Debug.Log("Video'd");
                InternalResetCall();

                if(UnityCallFactory.Instance.GetDefaultVideoDevice() != "")
                {
                    videoOn = true;
                    forceVideoOff = false;
                    SetupVideoUi(ConnectionId.INVALID);
                }
                mMediaConfig = CreateMediaConfig();
                mediaConfigUse = mMediaConfig;  
                    
                userAllowPermissions.allowVideo = true;              
                break;
            case "/videooff":
                Debug.Log("Video off'd");
                InternalResetCall();

                if(UnityCallFactory.Instance.GetDefaultVideoDevice() != "")
                {
                    videoOn = false;
                    forceVideoOff = false;
                    SetupVideoUi(ConnectionId.INVALID);
                }
                mMediaConfig = CreateMediaConfig();
                mediaConfigUse = mMediaConfig;  
                    
                userAllowPermissions.allowVideo = false;   
                break;
            default:
                break;
        }   
    }

    /// <summary>
    /// Sends a message to the other end
    /// </summary>
    /// <param name="msg"></param>
    public void SendMsg(string msg)
    {
        bool atBottomOfChat = false;

        if(!ExampleGlobals.HasAudioPermission())
        {
            Debug.Log("Permission not granted");
            return;
        }

        if (String.IsNullOrEmpty(msg))
        {
            //never send null or empty messages. webrtc can't deal with that
            return;
        }       

        Append(msg);

        if(isICall)
            iCall.Send(msg);

        //reset UI
        messageField.text = "";
        messageField.Select();
    }
    /// <summary>
    /// Adds a new message to the message view
    /// </summary>
    /// <param name="text"></param>
    private void Append(string text)
    {
        
        if (textOutput != null)
        {
            FloorChatIndexView();
            textOutput.AddTextEntry(text);
        }
        else
        {
            Debug.Log("Chat: " + text);
        }
    }

    public void ToggleChatVisibility()
    {
        lockChatVisibility = !lockChatVisibility;
    }

    public void FloorChatIndexView()
    {
        bool atBottomOfChat = false;
        if(scrollbar.value <= 0.1)
                atBottomOfChat = true;
        if(atBottomOfChat)
            StartCoroutine(SetScrollbar(3.0f));
    }

    private IEnumerator SetScrollbar(float value)
    {
        yield return new WaitForSeconds(Time.deltaTime * value);
        scrollbar.value = 0;
    }

    public void SetMuteSelf(bool status)
    {
        if(isMediaNetwork)
        {
            mediaNetwork.SetMute(status);
        } else if(isICall)
        {
            iCall.SetMute(status);
        }
    }

    public short GetRemoteUserID()
    {
        return remoteUserId.id;
    }

    public ConnectionId GetConnectionId()
    {
        return remoteUserId;
    }

    public void ButtonMuteChannel(ConnectionId remoteUser, float val)
    {
        if(isMediaNetwork)
        {
            mediaNetwork.SetVolume(val, remoteUser);
        } else if(isICall)
        {
            iCall.SetVolume(val, remoteUser);
        }
        
    }

    public void SetVolume(float volume, int user)
    {
        ConnectionId tempId;
        tempId = remoteUserId;
        tempId.id = (short)PhotonNetwork.LocalPlayer.ActorNumber;
        
        //iCall.SetVolume(volume, tempId);
    }
}
 
 
 

