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

    /// <summary>
    /// Helper to keep to keep track of each instance
    /// </summary>
    private static int sInstances = 0;

    /// <summary>
    /// Helper to give each instance an id to print via log output
    /// </summary>
    private int mIndex = 0;

    public InputField uMessageField;

    public ChatText uOutput;

    public GameObject receiveTxt;
    public bool lockChatVisibility = false;

    public Scrollbar scrollbar;

    protected bool mCallActive = false;
    protected string mUseAddress = null;
    private static string sAddress = null;
    private string sLocalAddress = null;
    protected MediaConfig mMediaConfigInUse;
    protected ConnectionId mRemoteUserId = ConnectionId.INVALID;
    public List<ConnectionId> mRemoteUserList = new List<ConnectionId>();
    private List<ConnectionId> mConnectionIds = new List<ConnectionId>();
    public bool startedVoiceServer = false;
    public List<int> connectedVoiceID = new List<int>();
    protected bool mAutoRejoin = true;
    protected IEnumerator mAutoRejoinCoroutine = null;
    protected float mRejoinTime = 0.2f;
    private float mediaNetworkConnectionTime = 2.0f;

    protected bool mLocalFrameEvents = true;

    public bool isConference = true;
    private bool isICall = false;
    private bool isMediaNetwork = true;

    
    private void Awake() {
        mMediaConfig = CreateMediaConfig();
        mMediaConfig.Video = false;
        mMediaConfigInUse = mMediaConfig;
    }

    void Start()
    {
        scrollbar.value = 0;

        StartCoroutine(ExampleGlobals.RequestPermissions(audioOn, videoOn));
        UnityCallFactory.EnsureInit(OnCallFactoryReadyMediaNetwork, OnCallFactoryFailedMediaNetwork);
        
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

    void Update()
    {
        if(isMediaNetwork)
        {
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
        }

        if (mCall != null && isICall)
        {
            mCall.Update();
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
                Debug.Log("Connection established");
                mRemoteUserId = ((CallAcceptedEventArgs)e).ConnectionId;
                mRemoteUserList.Add(mRemoteUserId);
                //mRemoteUserId.id = (short)69;
                //mRemoteUserId.id = (short)PhotonNetwork.LocalPlayer.ActorNumber;
                ConnectionId cID = new ConnectionId();
                cID.id = (short)2;
                //mCall.SetVolume(0, cID);
                Debug.Log("New connection with id: " + mRemoteUserId
                    + " audio:" + mCall.HasAudioTrack(mRemoteUserId)
                    + " video:" + mCall.HasVideoTrack(mRemoteUserId)
                    //+ " connection thing: " + mCall.SetVolume(0, cID)
                    + " New connection with id: " + mRemoteUserId.id);
                if(IgniteGameManager.IgniteInstance.gameTesting)
                    SendMsg(PhotonNetwork.NickName + ": New connection with id: " + mRemoteUserId);
                Debug.Log("Hash: " + mRemoteUserId.GetHashCode());
                Debug.Log(mRemoteUserId.GetHashCode());
                break;
            case CallEventType.CallEnded:
                //Call was ended / one of the users hung up -> reset the app
                Debug.Log("Call ended");
                mRemoteUserList.Clear();
                InternalResetCall();
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
                    if (e is FrameUpdateEventArgs)
                    {
                        //UpdateFrame((FrameUpdateEventArgs)e);
                    }
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

    protected virtual void UpdateFrame(FrameUpdateEventArgs frameUpdateEventArgs)
    {
        //the avoid wasting CPU time the library uses the format returned by the browser -> ABGR little endian thus
        //the bytes are in order R G B A
        //Unity seem to use this byte order but also flips the image horizontally (reading the last row first?)
        //this is reversed using UI to avoid wasting CPU time

        //Debug.Log("frame update remote: " + frameUpdateEventArgs.IsRemote);
        /*
        if (frameUpdateEventArgs.IsRemote == false)
        {
            mUi.UpdateLocalTexture(frameUpdateEventArgs.Frame, frameUpdateEventArgs.Format);
        }
        else
        {
            mUi.UpdateRemoteTexture(frameUpdateEventArgs.Frame, frameUpdateEventArgs.Format);
        }*/
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
            sLocalAddress = "Voice_" + PhotonNetwork.CurrentRoom.Name + "_User_" + "1";
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

            mMediaNetwork = UnityCallFactory.Instance.CreateMediaNetwork(netConfig);

            //keep track of multiple local instances for testing.
            mIndex = sInstances;
            sInstances++;
            Debug.Log("Instance " + mIndex + " created.");

            if (videoOn && audioOn)
            {
                //sender will broadcast audio and video
                mMediaConfig.Audio = audioOn;
                mMediaConfig.Video = videoOn;

                if(!startedVoiceServer)
                {
                    Debug.Log("Accepting incoming connections on " + sLocalAddress);
                    mMediaNetwork.Configure(mMediaConfig);
                    mMediaNetwork.StartServer(sLocalAddress);
                    mMediaNetwork.StartServer(sLocalAddress);
                    startedVoiceServer = true;
                }                
            } else if(audioOn && !videoOn)
            {
                mMediaConfig.Audio = audioOn;
                mMediaConfig.Video = videoOn;

                if(!startedVoiceServer)
                {
                    Debug.Log("Accepting incoming connections on " + sLocalAddress);
                    mMediaNetwork.Configure(mMediaConfig);
                    mMediaNetwork.StartServer(sLocalAddress);
                    startedVoiceServer = true;
                }    
            } else if(!audioOn && videoOn)
            {
                mMediaConfig.Audio = audioOn;
                mMediaConfig.Video = videoOn;

                if(!startedVoiceServer)
                {
                    Debug.Log("Accepting incoming connections on " + sLocalAddress);
                    mMediaNetwork.Configure(mMediaConfig);
                    mMediaNetwork.StartServer(sLocalAddress);
                    startedVoiceServer = true;
                }    
            } else
            {
                //this one will just receive (but could also send if needed)
                mMediaConfig.Audio = false;
                mMediaConfig.Video = false;
                if(!startedVoiceServer)
                {
                    mMediaNetwork.Configure(mMediaConfig);
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
                    mMediaNetwork.Connect(sAddress);
                    connectedVoiceID.Add(playerActorID);
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
        if(mMediaNetwork != null)
        {
            mMediaNetwork.Dispose();
            mMediaNetwork = null;
            startedVoiceServer = false;
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
    #endregion MediaNetwokr
    

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
        //mCall.Send(msg);



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
        if(isMediaNetwork)
        {
            mMediaNetwork.SetMute(status);
        } else if(isICall)
        {
            mCall.SetMute(status);
        }
            
        
    }

    public short GetRemoteUserID()
    {
        return mRemoteUserId.id;
    }

    public ConnectionId GetConnectionId()
    {
        return mRemoteUserId;
    }

    public void ButtonMuteChannel(ConnectionId remoteUser, float val)
    {
        if(isMediaNetwork)
        {
            mMediaNetwork.SetVolume(val, remoteUser);
        } else if(isICall)
        {
            mCall.SetVolume(val, remoteUser);
        }
        
    }

    public void GetHash(int userIndex)
    {
        Debug.Log("this mcall hash: " + mCall.GetHashCode());
        Debug.Log("Targetted cid hash: " + mRemoteUserList[userIndex].GetHashCode());
        Debug.Log("Targetted cid string: " + mRemoteUserList[userIndex].ToString());
        Debug.Log("Targetted cid buffered amount?: " + mCall.GetBufferedAmount(mRemoteUserList[userIndex], true));
    }

    public void SetVolume(float volume, int user)
    {
        ConnectionId tempId;
        tempId = mRemoteUserId;
        tempId.id = (short)PhotonNetwork.LocalPlayer.ActorNumber;
        
        //mCall.SetVolume(volume, tempId);
    }
}
