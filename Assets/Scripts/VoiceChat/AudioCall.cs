using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Byn.Awrtc;
using Byn.Awrtc.Unity;
using Byn.Unity.Examples;
using UnityEngine.UI;

//Reference CallApp.js for more info
public class AudioCall : MonoBehaviourPunCallbacks
{
    public string voiceID = "";
    public AudioCallUI audioCallUI;

    public string uSignalingUrl = "ws://signaling.because-why-not.com/callapp";

    public string uSecureSignalingUrl = "wss://signaling.because-why-not.com/callapp";


    public bool uForceSecureSignaling = false;

    public string uIceServer = "stun:stun.because-why-not.com:443";

    //
    public string uIceServerUser = "";
    public string uIceServerPassword = "";
    public string uIceServer2 = "stun:stun.l.google.com:19302";
    public string uIceServer3 = "stun:stun.l.google.com:19302";
    public string uIceServer4 = "stun:stun.l.google.com:19302";
    public string uIceServer5 = "stun:stun.l.google.com:19302";
    public string uIceServer6 = "stun:stun.l.google.com:19302";
    public string uIceServer7 = "stun:stun.l.google.com:19302";
    
    public const int MAX_CODE_LENGTH = 256;

    protected ICall mCall;

    private IMediaNetwork mMediaNetwork = null;



    public bool audioOn = true;
    public bool videoOn = false;

    
    protected MediaConfig mMediaConfig;

    public InputField uMessageField;

    public ChatText uOutput;

    public GameObject receiveTxt;
    public bool lockChatVisibility = false;

    public Scrollbar scrollbar;

    protected bool mCallActive = false;
    protected string mUseAddress = null;
    private static string sAddress = null;
    protected MediaConfig mMediaConfigInUse;
    protected ConnectionId mRemoteUserId = ConnectionId.INVALID;
    private List<ConnectionId> mConnectionIds = new List<ConnectionId>();
    protected bool mAutoRejoin = true;
    protected IEnumerator mAutoRejoinCoroutine = null;
    protected float mRejoinTime = 0.2f;

    protected bool mLocalFrameEvents = true;

    public bool isConference = true;

    public GameObject uVideoLayout;
    public GameObject uVideoPrefab;

    public Texture2D uNoImgTexture;

    private class VideoData
    {
        public GameObject uiObject;
        public Texture2D texture;
        public RawImage image;

    }

    private Dictionary<ConnectionId, VideoData> mVideoUiElements = new Dictionary<ConnectionId, VideoData>();
    
    private void Awake() {
        mMediaConfig = CreateMediaConfig();
        mMediaConfig.Video = false;
        mMediaConfigInUse = mMediaConfig;
    }

    void Start()
    {
        scrollbar.value = 0;

        StartCoroutine(ExampleGlobals.RequestPermissions(audioOn, videoOn));
        UnityCallFactory.EnsureInit(OnCallFactoryReady, OnCallFactoryFailed);
        
         //Disabling ICall Events
        InvokeRepeating("GetRoomID", 1.0f, 1.0f);

        //in case we have any UI we want to add in.
        if(GetComponent<AudioCallUI>())
        {
            audioCallUI = GetComponent<AudioCallUI>();
        }
        StartCoroutine(ExampleGlobals.RequestPermissions(true, false));
        UnityCallFactory.EnsureInit(OnCallFactoryReady, OnCallFactoryFailed);
    }

    void Update()
    {

        /*
        if (mMediaNetwork == null)
            return;

        mMediaNetwork.Update();

        //This is the event handler via polling.
        //This needs to be called or the memory will fill up with unhanded events!
        NetworkEvent evt;
        while (mMediaNetwork != null && mMediaNetwork.Dequeue(out evt))
        {
            HandleNetworkEvent(evt);
        }
        //polls for video updates
        HandleMediaEvents();

        //Flush will resync changes done in unity to the native implementation
        //(and possibly drop events that aren't handled in the future)
        if (mMediaNetwork != null)
            mMediaNetwork.Flush();
            */
        
        if (mCall != null)
        {
            mCall.Update();
        }
        
    }

    //-----------------ICALL -------------------------
    
    protected virtual void OnCallFactoryReady()
    {
        //set to warning for regular use
        UnityCallFactory.Instance.RequestLogLevel(UnityCallFactory.LogLevel.Info);
    }

    protected virtual void OnCallFactoryFailed(string error)
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

        mediaConfig.Audio = true;
        mediaConfig.Video = false;
        mediaConfig.VideoDeviceName = null;

        //This format is the only reliable format that works on all
        //platforms currently.
        mediaConfig.Format = FramePixelFormat.ABGR;

        /* original lines
        mediaConfig.MinWidth = 160;
        mediaConfig.MinHeight = 120;
        mediaConfig.MaxWidth = 1920;
        mediaConfig.MaxHeight = 1080;

        //will be overwritten by UI in normal use
        mediaConfig.IdealWidth = 160;
        mediaConfig.IdealHeight = 120;
        mediaConfig.IdealFrameRate = 30;
        */

        mediaConfig.MinWidth = 0;
        mediaConfig.MinHeight = 0;
        mediaConfig.MaxWidth = 0;
        mediaConfig.MaxHeight = 0;
        mediaConfig.IdealWidth = 0;
        mediaConfig.IdealHeight = 0;
        mediaConfig.IdealFrameRate = 30;

        return mediaConfig;
    }

    public void SetupCall()
    {
        Debug.Log("Setting up ...");

        NetworkConfig netConfig = CreateNetworkConfig();
        //this is what we need for multiple audio connections
        netConfig.IsConference = true;

        Debug.Log("Creating call using NetworkConfig:" + netConfig);
        mCall = CreateCall(netConfig);
        if (mCall == null)
        {
            Debug.Log("Failed to create the call");
            return;
        }

        mCall.LocalFrameEvents = mLocalFrameEvents;
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
        mCall.CallEvent += Call_CallEvent;



        //make a deep clone to avoid confusion if settings are changed
        //at runtime. 
        mMediaConfigInUse = mMediaConfig.DeepClone();

        //try to pick a good default video device if the user wants to send video but
        //didn't bother to pick a specific device
        if (mMediaConfigInUse.Video && string.IsNullOrEmpty(mMediaConfigInUse.VideoDeviceName))
        {
            mMediaConfigInUse.VideoDeviceName = UnityCallFactory.Instance.GetDefaultVideoDevice();
        }

        Debug.Log("Configure call using MediaConfig: " + mMediaConfigInUse);
        mCall.Configure(mMediaConfigInUse);
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

                mRemoteUserId = ((CallAcceptedEventArgs)e).ConnectionId;
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
                    mCall.Call(mUseAddress);
                } else {
                    mCall.Listen(mUseAddress);
                }*/
                mCall.Call(mUseAddress);
                    
                
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
        vd.uiObject = Instantiate(uVideoPrefab);
        vd.uiObject.transform.SetParent(uVideoLayout.transform, false);
        vd.image = vd.uiObject.GetComponentInChildren<RawImage>();
        vd.image.texture = uNoImgTexture;
        mVideoUiElements[id] = vd;
    }

    private void UpdateFrame(ConnectionId id, IFrame frame)
    {
        if (mVideoUiElements.ContainsKey(id))
        {
            VideoData videoData = mVideoUiElements[id];
            UpdateTexture(ref videoData.texture, frame);
            videoData.image.texture = videoData.texture;
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
            ///copy image data into the texture and apply
            tex.LoadRawTextureData(frame.Buffer);
            tex.Apply();
        }


    private void InternalResetCall()
    {
        CleanupCall();
        if (mAutoRejoin)
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
        if (mCall != null)
        {
            mCallActive = false;
            mRemoteUserId = ConnectionId.INVALID;
            Debug.Log("Destroying call!");
            mCall.CallEvent -= Call_CallEvent;
            mCall.Dispose();
            mCall = null;
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
        Debug.Log("Restarting in " + mRejoinTime + " seconds!");
        mAutoRejoinCoroutine = CoroutineRejoin();
        StartCoroutine(mAutoRejoinCoroutine);
    }

    private IEnumerator CoroutineRejoin()
    {
        yield return new WaitForSecondsRealtime(mRejoinTime);
        SetupCall();
        InternalJoin();
    }

    private void InternalJoin()
    {
        if (mCallActive)
        {
            Debug.LogError("Join call failed. Call is already/still active");
            return;
        }
        Debug.Log("Try listing on address: " + mUseAddress);
        if(IgniteGameManager.IgniteInstance.gameTesting)
            SendMsg(PhotonNetwork.NickName + ": " + "Try listing on address: " + mUseAddress);
        mCallActive = true;
        this.mCall.Listen(mUseAddress);
    }

    public virtual void Join(string address)
    {
        if (address.Length > MAX_CODE_LENGTH)
            throw new ArgumentException("Address can't be longer than " + MAX_CODE_LENGTH);
        mUseAddress = address;
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
    //-----------------ICALL END----------------------

    //-----------------MediaNetwork-------------------
    /*
    protected virtual void OnCallFactoryReady()
    {
        UnityCallFactory.Instance.RequestLogLevel(UnityCallFactory.LogLevel.Info);
        StartCoroutine(InitWebRTC());
    }

    protected virtual void OnCallFactoryFailed(string error)
    {
        string fullErrorMsg = typeof(CallApp).Name + " can't start. The " + typeof(UnityCallFactory).Name + " failed to initialize with following error: " + error;
        Debug.LogError(fullErrorMsg);
    }*/

    /// <summary>
    /// Init process. Sets configuration and triggers the connection process
    /// </summary>
    /// <returns>
    /// Returns IEnumerator so unity treats it as a Coroutine
    /// </returns>
    private IEnumerator InitWebRTC()
    {
        if (sAddress == null)
        {
            //to avoid the awkward moment of connecting two random users who test this package
            //we use a randomized addresses now to connect only the local test Apps  ;)
            sAddress = "Voice_" + PhotonNetwork.CurrentRoom.Name + "_User_" + PhotonNetwork.LocalPlayer.ActorNumber;

        }


        if (UnityCallFactory.Instance == null)
        {
            Debug.LogError("No access to webrtc. ");
        }
        else
        {
            UnityCallFactory.Instance.RequestLogLevel(UnityCallFactory.LogLevel.Info);
            //Factory works. Prepare Peers
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

            mMediaNetwork = UnityCallFactory.Instance.CreateMediaNetwork(netConfig);

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

                Debug.Log("Accepting incoming connections on " + sAddress);
                mMediaNetwork.Configure(mMediaConfig);
                mMediaNetwork.StartServer(sAddress);
            } else if(audioOn && !videoOn)
            {
                mMediaConfig.Audio = audioOn;
                mMediaConfig.Video = videoOn;

                Debug.Log("Accepting incoming connections on " + sAddress);
                mMediaNetwork.Configure(mMediaConfig);
                mMediaNetwork.StartServer(sAddress);
            } else if(!audioOn && videoOn)
            {
                mMediaConfig.Audio = audioOn;
                mMediaConfig.Video = videoOn;

                Debug.Log("Accepting incoming connections on " + sAddress);
                mMediaNetwork.Configure(mMediaConfig);
                mMediaNetwork.StartServer(sAddress);
            } else
            {
                //this one will just receive (but could also send if needed)
                mMediaConfig.Audio = false;
                mMediaConfig.Video = false;
                mMediaNetwork.Configure(mMediaConfig);
            }
            //wait a while before trying to connect othe sender
            //so it has time to register at the signaling server
            yield return new WaitForSeconds(mRejoinTime);
            if (videoOn == false)
            {
                Debug.Log("Tring to connect to " + sAddress);
                mMediaNetwork.Connect(sAddress);
            }
        }
    }

    private void OnDestroy()
    {
        //Destroy the network
        if (mMediaNetwork != null)
        {
            mMediaNetwork.Dispose();
            mMediaNetwork = null;
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

        if (mMediaNetwork != null && handleLocalFrames)
        {
            IFrame localFrame = mMediaNetwork.TryGetFrame(ConnectionId.INVALID);
            if (localFrame != null)
            {

            }
        }
        if (mMediaNetwork != null && handleRemoteFrames)
        {
            //so far the loop shouldn't be needed. we only expect one
            foreach (var id in mConnectionIds)
            {
                if (mMediaNetwork != null)
                {
                    IFrame remoteFrame = mMediaNetwork.TryGetFrame(id);
                    if (remoteFrame != null)
                    {

                    }
                }
            }
        }
    }

    /// <summary>
    /// Log method to help seeing what each of the different apps does.
    /// </summary>
    /// <param name="txt"></param>
    private void Log(string txt)
    {
        Debug.Log("Instance " + PhotonNetwork.LocalPlayer.ActorNumber + ": " + txt);
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

                mConnectionIds.Add(evt.ConnectionId);
                Log("New connection id " + evt.ConnectionId);

                break;
            case NetEventType.ConnectionFailed:
                //call failed
                Log("Outgoing connection failed");
                break;
            case NetEventType.Disconnected:

                if (mConnectionIds.Contains(evt.ConnectionId))
                {
                    mConnectionIds.Remove(evt.ConnectionId);

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
    //-----------------MediaNetwork END---------------
    

    /// <summary>
    /// This is called if the send button
    /// </summary>
    public void SendButtonPressed()
    {
        //get the message written into the text field
        string msg = PhotonNetwork.NickName + ": " + uMessageField.text;
        
        SendMsg(msg);
    }
    /// <summary>
    /// Sends a message to the other end
    /// </summary>
    /// <param name="msg"></param>
    public void SendMsg(string msg)
    {
        bool atBottomOfChat = false;
        if (String.IsNullOrEmpty(msg))
        {
            //never send null or empty messages. webrtc can't deal with that
            return;
        }        
            
        Append(msg);
        
        mCall.Send(msg);



        //reset UI
        uMessageField.text = "";
        uMessageField.Select();
    }
    /// <summary>
    /// Adds a new message to the message view
    /// </summary>
    /// <param name="text"></param>
    private void Append(string text)
    {
        
        if (uOutput != null)
        {
            FloorChatIndexView();
            uOutput.AddTextEntry(text);
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
        mCall.SetMute(status);
    }

    public short GetRemoteUserID()
    {
        return mRemoteUserId.id;
    }

    public ConnectionId GetConnectionId()
    {
        return mRemoteUserId;
    }

    public void SetVolume(float volume, int user)
    {
        ConnectionId tempId;
        tempId = mRemoteUserId;
        tempId.id = (short)PhotonNetwork.LocalPlayer.ActorNumber;
        
        //mCall.SetVolume(volume, tempId);
    }
}
