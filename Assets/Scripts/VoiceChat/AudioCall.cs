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
    
    protected MediaConfig mMediaConfig;

    public InputField uMessageField;

    public ChatText uOutput;

    public GameObject receiveTxt;
    public bool lockChatVisibility = false;

    public Scrollbar scrollbar;

    protected bool mCallActive = false;
    protected string mUseAddress = null;
    protected MediaConfig mMediaConfigInUse;
    protected ConnectionId mRemoteUserId = ConnectionId.INVALID;
    protected bool mAutoRejoin = true;
    protected IEnumerator mAutoRejoinCoroutine = null;
    protected float mRejoinTime = 0.2f;

    protected bool mLocalFrameEvents = true;

    public bool isConference = true;

    private void Awake() {
        mMediaConfig = CreateMediaConfig();
        mMediaConfig.Video = false;
        mMediaConfigInUse = mMediaConfig;
        
    }

    void Start()
    {
        scrollbar.value = 0;
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
        if (mCall != null)
        {
            mCall.Update();
        }
    }

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
        mediaConfig.IdealFrameRate = 30;*/

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
                mRemoteUserId.id = (short)PhotonNetwork.LocalPlayer.ActorNumber;
                
                Debug.Log("New connection with id: " + mRemoteUserId
                    + " audio:" + mCall.HasAudioTrack(mRemoteUserId)
                    + " video:" + mCall.HasVideoTrack(mRemoteUserId)
                    + "New connection with id: " + mRemoteUserId.id);
                break;
            case CallEventType.CallEnded:
                //Call was ended / one of the users hung up -> reset the app
                Debug.Log("Call ended");
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
    private void SendMsg(string msg)
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
        
        mCall.SetVolume(volume, tempId);
    }
}
