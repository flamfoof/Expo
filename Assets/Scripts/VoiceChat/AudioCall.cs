using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
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
    protected MediaConfig mediaConfig;
    private bool isICall = false;
    private bool isMediaNetwork = true;
    public bool startedVoiceServer = false;
    public InputField messageField;
    public ChatText textOutput;
    public GameObject receiveTxt;
    public bool lockChatVisibility = false;
    public Scrollbar scrollbar;
    protected bool iCallActive = false;
    private string localAddress = null;
    protected string targetAddress = null;
    protected MediaConfig mediaConfigUse;
    protected ConnectionId remoteUserId = ConnectionId.INVALID;
    private List<ConnectionId> connectionIdList = new List<ConnectionId>();
    protected bool autoRejoin = true;
    protected IEnumerator autoRejoinCoroutine = null;
    protected float rejoinTime = 0.2f;
    private float mediaNetworkConnectionTime = 4.0f;
    protected bool localFrameEvents = true;
    public bool isConference = true;
    public GameObject[] videoImage;
    private Texture2D videoTextureBuffer;
    public List<int> idWithVideos = new List<int>();
    public List<int> connectedVoiceID = new List<int>();
    public bool reconnectingVoiceCoroutine = false;
    public float noVidWaitTime = 4.0f;
    public bool hasFrame;
    public float noTextureTimer;
    
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
    public struct SConnectionMapping
    {
        public int playerID;
        public int connectionId;
        public ConnectionId cID;
        public bool forceReconnect;
    }
    
    SConnectionMapping thisConnectionMap = new SConnectionMapping();
    private Dictionary<ConnectionId, VideoData> mVideoUiElements = new Dictionary<ConnectionId, VideoData>();
    public Dictionary<ConnectionId, int> connectionMappingDict = new Dictionary<ConnectionId, int>();
    public Dictionary<int, ConnectionId> connectionMappingDictToUsers = new Dictionary<int, ConnectionId>();
    public Dictionary<int, ConnectionId> connectionMappingDictToUsersBackUp = new Dictionary<int, ConnectionId>();
    
    #if UNITY_WEBGL
    private void Awake() {
        userAllowPermissions = UserPermissionCommunication.instance;
        mediaConfig = CreateMediaConfig();
        mediaConfigUse = mediaConfig;   

        if(isMediaNetwork)
        {
            UnityCallFactory.EnsureInit(OnCallFactoryReadyMediaNetwork, OnCallFactoryFailedMediaNetwork);
        }
        thisConnectionMap.playerID = PhotonNetwork.LocalPlayer.ActorNumber;
        thisConnectionMap.connectionId = -2;
    }
    #endif
    
    void Start()
    {
        scrollbar.value = 0;
        mediaConfig = CreateMediaConfig();
        mediaConfigUse = mediaConfig; 
        userAllowPermissions = UserPermissionCommunication.instance;
        //StartCoroutine(ExampleGlobals.RequestPermissions(audioOn, videoOn));

        //in case we have any UI we want to add in.
        if(GetComponent<AudioCallUI>())
        {
            audioCallUI = GetComponent<AudioCallUI>();
        }
    
        StartCoroutine(UpdateNoTexture());
        StartCoroutine(AutoReconnectVoiceId(autoRejoin));
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

#if UNITY_EDITOR
            if(mediaNetwork.Peek(out evt))
            {                
                try
                {
                    if(evt.Type != NetEventType.ReliableMessageReceived && evt.Type != NetEventType.NewConnection && evt.Type != NetEventType.UnreliableMessageReceived)
                    Debug.Log("ERROR IN SERVER TYPE: " + evt.Type);
                    Debug.Log("ERROR IN SERVER TYPE INFO: " + evt.Info);
                    Debug.Log("ERROR IN SERVER: " + evt.ErrorInfo.ToString());
                } catch (Exception e)
                {
                    Debug.Log("No reports");
                }                   
            }
#endif

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
        //Debug.Log("Example vid dev is: " + UnityCallFactory.Instance.GetDefaultVideoDevice());
        if(userAllowPermissions)
        {
            //Debug.Log("Video perm is defined: " + userAllowPermissions.allowVideo);
            if(UnityCallFactory.Instance.GetDefaultVideoDevice() != "" && userAllowPermissions.allowVideo)
            {
                //Debug.Log("There exists a video thing and user allowed it.");
                videoOn = true;   
            }
        }
        
        
        mediaConfig.Audio = audioOn;
        //Debug.Log("Audio Perm is: " + mediaConfig.Audio);
        if(forceVideoOff)
        {
            mediaConfig.Video = false;
        } else
        {
            mediaConfig.Video = videoOn;
        }
        
        //Debug.Log("Video Perm is: " + mediaConfig.Video);
        mediaConfig.VideoDeviceName = UnityCallFactory.Instance.GetDefaultVideoDevice();
        mediaConfig.Format = FramePixelFormat.ABGR;
        mediaConfig.MinWidth = 160;
        mediaConfig.MinHeight = 120;
        mediaConfig.MaxWidth = 1920;
        mediaConfig.MaxHeight = 1080;
        mediaConfig.IdealWidth = 160;
        mediaConfig.IdealHeight = 120;
        mediaConfig.IdealFrameRate = 30;
        return mediaConfig;
    }
    
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
        if (targetAddress == null)
        {
            targetAddress = "Voice_" + PhotonNetwork.CurrentRoom.Name + "_User_" + playerActorID;
            Debug.Log("PLAYER IS: " + targetAddress);
            localAddress = "Voice_" + PhotonNetwork.CurrentRoom.Name + "_User_" + PhotonNetwork.LocalPlayer.ActorNumber;
        }

        if (UnityCallFactory.Instance == null)
        {
            Debug.LogError("No access to webrtc. ");
        } else if (connectedVoiceID.Contains(playerActorID))
        {
            Debug.Log("This connection is already being used");
            targetAddress = null;
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
            }
            
            if (string.IsNullOrEmpty(uIceServer3) == false)
            {
                netConfig.IceServers.Add(new IceServer(uIceServer3));
                //Debug.Log("Connected to RTC: " + uIceServer3);
            }

            /*
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
            }*/
            

            if (Application.platform == RuntimePlatform.WebGLPlayer || uForceSecureSignaling)
            {
                netConfig.SignalingUrl = uSecureSignalingUrl;
            }
            else
            {
                netConfig.SignalingUrl = uSignalingUrl;
            }

            mediaNetwork = UnityCallFactory.Instance.CreateMediaNetwork(netConfig);
            Debug.Log(videoOn);

            Debug.Log("Example vid dev is: " + UnityCallFactory.Instance.GetDefaultVideoDevice());
            if(userAllowPermissions)
            {
                Debug.Log("Video perm is defined: " + userAllowPermissions.allowVideo);
                if(UnityCallFactory.Instance.GetDefaultVideoDevice() != "" && userAllowPermissions.allowVideo)
                {
                    Debug.Log("There exists a video device and user allowed it.");
                    videoOn = true;   
                }
            }

            Debug.Log(videoOn);

            mediaConfig.Audio = audioOn;
            //Debug.Log("Audio Perm is: " + mediaConfig.Audio);

            if(forceVideoOff)
            {
                mediaConfig.Video = false;
                videoOn = false;
            } else
            {
                mediaConfig.Video = videoOn;
            }

            Debug.Log(videoOn);
            //Debug.Log("Video Perm is: " + mediaConfig.Video);
            
            if (videoOn || audioOn)
            {
                //sender will broadcast audio and video
                if(!startedVoiceServer)
                {
                    Debug.Log("Accepting incoming connections on " + localAddress + " with audio: " + mediaConfig.Audio + " video: " + mediaConfig.Video);
                    mediaNetwork.Configure(mediaConfig);
                    mediaNetwork.StartServer(localAddress);
                    startedVoiceServer = true;
                }               
            } else
            {
                //this one will just receive (but could also send if needed)
                mediaConfig.Audio = false;
                mediaConfig.Video = false;
                if(!startedVoiceServer)
                {
                    mediaNetwork.Configure(mediaConfig);
                    startedVoiceServer = true;
                }   
            }
            //wait a while before trying to connect othe sender
            //so it has time to register at the signaling server
            yield return new WaitForSeconds(mediaNetworkConnectionTime);           
            /*
            if(!connectedVoiceID.Contains(playerActorID))
            {
                Debug.Log("<color=white>Trying to connect to: </color>" + targetAddress);
                mediaNetwork.Connect(targetAddress);
                
                connectedVoiceID.Add(playerActorID);
                targetAddress = null;
            } else
            {
                Debug.Log("The address has already been connected to: " + targetAddress);
                targetAddress = null;
            }         */  
        }
    }

    public void ReconnectAllVoiceID()
    {
        Photon.Realtime.Player[] playerList = PhotonNetwork.PlayerListOthers;
        for(int i = 0; i < playerList.Length; i++)
        {
            ConnectToPlayerVoice(playerList[i].ActorNumber);
        }
    }

    public IEnumerator AutoReconnectVoiceId(bool autoOn)
    {
        while(autoOn)
        {
            yield return new WaitForSeconds(mediaNetworkConnectionTime);
            //Debug.Log("Reconnecting automatically");
            if(mediaNetwork != null)
            {
                ReconnectAllVoiceID();  
            }
            //MapConnectionsToDictRefreshUnique();    
        }
            
    }

    public void ConnectToPlayerVoice(int playerActorID)
    {
        string address = "Voice_" + PhotonNetwork.CurrentRoom.Name + "_User_" + playerActorID;
        if (UnityCallFactory.Instance == null)
        {
            Debug.LogError("No access to webrtc.");
        } else if (connectionMappingDictToUsers.ContainsKey(playerActorID))
        {
            //Debug.Log("This connection is already being used");
            targetAddress = null;
        } else
        {
            if(!connectionMappingDictToUsers.ContainsKey(playerActorID))
            {
                Debug.Log("<color=yellow>Trying to connect to: </color>" + address);
                mediaNetwork.Connect(address);
                targetAddress = null;
            } else
            {
                Debug.Log("The address has already been connected to: " + address);
                targetAddress = null;
            }   
        }
    }

    public void DisconnectPlayerVoice(ConnectionId cID)
    {       
        mediaNetwork.Disconnect(cID);
    }

    public IEnumerator ReconnectToPlayerVoice(int playerActorID)
    {
        yield return new WaitForSeconds(mediaNetworkConnectionTime);
        string address = "Voice_" + PhotonNetwork.CurrentRoom.Name + "_User_" + playerActorID;
        //Debug.Log("Trying to REconnect to other player: " + address);
        if (UnityCallFactory.Instance == null)
        {
            Debug.LogError("No access to webrtc. ");
        } else
        {            
            Debug.Log("<color=orange>Trying to REconnect to </color>" + address);
            mediaNetwork.Connect(address);
            targetAddress = null;
        }
        reconnectingVoiceCoroutine = false;
    }

    private void MediaNetworkDispose()
    {
        if(mediaNetwork != null)
        {
            mediaNetwork.Dispose();
            mediaNetwork = null;
            if(connectedVoiceID.Count > 0)
                connectedVoiceID.Clear();
            if(connectionIdList.Count > 0)
                connectionIdList.Clear();
            UnityCallFactory.Instance.Dispose();
            
            
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
            if(connectedVoiceID.Count > 0)
                connectedVoiceID.Clear();
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
        hasFrame = false;

        //check if connection id has video track
        if (mediaNetwork != null && handleLocalFrames)  
        {
            IFrame localFrame = mediaNetwork.TryGetFrame(ConnectionId.INVALID);
            if (localFrame != null)
            {
                hasFrame = true;
                UpdateTexture(localFrame);
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
                        hasFrame = true;
                        UpdateTexture(remoteFrame);
                    }
                }
            }
        }
    }
    
    private IEnumerator UpdateNoTexture()
    {
        if(videoImage.Length > 0)
        {
            while(true)
            {
                yield return new WaitForFixedUpdate();
                if(!hasFrame)
                {
                    if(noTextureTimer > noVidWaitTime)
                    {
                        for(int i = 0; i < videoImage.Length; i++)
                        {
                            videoImage[i].GetComponent<RawImage>().texture = noVidTexture;
                        }
                        
                    } else
                    {
                        noTextureTimer += Time.fixedDeltaTime;
                    } 
                } else
                {
                    noTextureTimer = 0.0f;
                }
            }  
        }   
    }

    private void UpdateTexture(IFrame frame)
    {
        //TODO: Make the texture render on the correct video image if there are multiple
        if (videoImage != null)
        {
            if (frame != null)
            {
                UpdateTexture(ref videoTextureBuffer, frame);
                for(int i = 0; i < videoImage.Length; i++)
                {
                    videoImage[i].GetComponent<RawImage>().texture = videoTextureBuffer;
                }
                
            }
        }
    }

    /// <summary>
    /// Wrties the raw frame into the given texture or creates it if null or wrong width/height.
    /// </summary>
    /// <param name="tex"></param>
    /// <param name="frame"></param>
    /// <returns></returns>
    protected bool UpdateTexture(ref Texture2D tex, IFrame frame)
    {
        bool newTextureCreated = false;
        //texture exists but has the wrong height /width? -> destroy it and set the value to null
        if (tex != null && (tex.width != frame.Width || tex.height != frame.Height))
        {
            Texture2D.Destroy(tex);
            tex = null;
        }
        //no texture? create a new one first
        if (tex == null)
        {
            newTextureCreated = true;
            Debug.Log("Creating new texture with resolution " + frame.Width + "x" + frame.Height + " Format:" + mediaConfig.Format);
            if (mediaConfigUse.Format == FramePixelFormat.ABGR)
            {
                tex = new Texture2D(frame.Width, frame.Height, TextureFormat.RGBA32, false);
            }
            else
            {
                //not yet properly supported.
                tex = new Texture2D(frame.Width, frame.Height, TextureFormat.YUY2, false);
            }
            tex.wrapMode = TextureWrapMode.Clamp;
        }
        ///copy image data into the texture and apply
        tex.LoadRawTextureData(frame.Buffer);
        tex.Apply();
        return newTextureCreated;
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
                Log("New connection id " + evt.ConnectionId);   
                connectionIdList.Add(evt.ConnectionId);
                //SendMessageString(thisConnectionMap);
                try
                {                   
                    Log("<color=yellow>Trying to PM and map new connection id at: </color>" + evt.ConnectionId.id);
                    if(!connectedVoiceID.Contains(evt.ConnectionId.id))
                    {
                        connectedVoiceID.Add(evt.ConnectionId.id);
                        SConnectionMapping tempConnection = new SConnectionMapping();
                        tempConnection.playerID = PhotonNetwork.LocalPlayer.ActorNumber;
                        tempConnection.connectionId = evt.ConnectionId.id;
                        tempConnection.cID = evt.ConnectionId;
                        SendPM(tempConnection);
                    }
                } catch (Exception e)
                {
                    Log("Unable to connect to incoming connection because of: " + e);
                }

                break;
            case NetEventType.ConnectionFailed:
                //call failed
                Log("<color=red>Outgoing connection failed. Retrying to connect to: </color>" + evt.ConnectionId.id);
                Log("<color=red>Failed because: </color>" + evt.ErrorInfo);
                //Log("<color=red>Outgoing connection failed. Retrying to connect to: </color>" + evt.Info);
                if(connectionMappingDict.TryGetValue(evt.ConnectionId, out int value))
                {
                    StartCoroutine(ReconnectToPlayerVoice(value));
                    
                    Debug.Log("Remaining Dictionary Count: " + connectionMappingDict.Count);
                    foreach(KeyValuePair<ConnectionId, int> kvp in connectionMappingDict)
                    {
                        Log("<color=blue> Connection id: " + kvp.Key + "    Player Id: " + kvp.Value +"</color>");
                    }
                    MapConnectionsToDictRemove(evt.ConnectionId);
                } else
                {                    
                    Log("Unable to get correct mapping of connection id for: " + evt.ConnectionId.id);
                    connectionIdList.Remove(evt.ConnectionId);
                    connectedVoiceID.Remove(evt.ConnectionId.id);
                }
                /*
                if(!connectedVoiceID.Contains(evt.ConnectionId.id))
                {
                    Log("Connection ID currently doesn't exist: " + evt.ConnectionId.id);
                } else
                {
                    Log("Currently busy trying to reconnect to another voice id");
                }*/
                    
                //ConnectToPlayerVoice(evt.ConnectionId.id);
                break;
            case NetEventType.Disconnected:
                if (connectionIdList.Contains(evt.ConnectionId))
                {
                    connectionIdList.Remove(evt.ConnectionId);
                    connectedVoiceID.Remove(evt.ConnectionId.id);
                    MapConnectionsToDictRemove(evt.ConnectionId);
                    
                    //Append("You have been disconnected from: " + PhotonNetwork.GetPhotonView)
                }
                Log("Connection disconnected on: " + evt.ConnectionId.id);
                break;
            case NetEventType.ServerInitialized:
                //incoming calls possible
                Log("Server ready for incoming connections. Address: " + evt.Info);
                ReconnectAllVoiceID();
                break;
            case NetEventType.ServerInitFailed:
                Log("Server init failed");
                break;
            case NetEventType.ServerClosed:
                Log("Server stopped");
                break;
            case NetEventType.ReliableMessageReceived:
                HandleIncommingMessage(ref evt);
                break;
            case NetEventType.UnreliableMessageReceived:
            {
                HandleIncommingMessage(ref evt);
                break;
            }
            default:
                break;
        }
    }

    private void HandleIncommingMessage(ref NetworkEvent evt)
    {
        MessageDataBuffer buffer = (MessageDataBuffer)evt.MessageData;
        SConnectionMapping connectionMapping = new SConnectionMapping();
        string msg = Encoding.UTF8.GetString(buffer.Buffer, 0, buffer.ContentLength);
        bool isChatText = true;
        Log("Received message from: " + evt.ConnectionId.id + " with: " + msg);
        try
        {
            connectionMapping = JsonUtility.FromJson<SConnectionMapping>(msg);
            isChatText = false;
            Debug.Log("Json SConnection Mapping output: " + connectionMapping.connectionId + " and player id is: " + connectionMapping.playerID);
            MapConnectionsToDictAdd(connectionMapping.playerID, evt.ConnectionId);
            if(connectionMapping.forceReconnect)
            {
                Debug.Log("Force reconnecting");
                /*
                foreach(KeyValuePair<int, ConnectionId> kvp in connectionMappingDictToUsers)
                {
                    ConnectToPlayerVoice(kvp.Key);
                }*/
                ReconnectAllVoiceID();
            }
        } catch (Exception e)
        {
            if(e != null)
            {
                //Debug.Log("Error happened here when parsing json: " + e);
            }
            //Debug.Log("Not a connection mapping");
        }
        
        if(!isChatText)
        {
            return;
        }
        //if server -> forward the message to everyone else including the sender
        if (startedVoiceServer && isChatText)
        {
            //we use the server side connection id to identify the client
            string idAndMessage = msg;
            //SendPlayerMessage();
            Append(idAndMessage);
        }
        else
        {
            //client received a message from the server -> simply print
            Debug.Log("Simple printed message from server");
            Append(msg);
        }
        //return the buffer so the network can reuse it
        buffer.Dispose();
    }

    //reliable to true at all times because we always want messages to get across 100%
    public void SendPlayerMessage(bool reliable = true)
    {
        string msg = PhotonNetwork.NickName + ": " + messageField.text;
        string cmd = messageField.text;
        if(cmd[0] == (char)'/')
        {
            Debug.Log("Chat command activated");
            ChatCommand(messageField.text);
            return;
        }
        if (mediaNetwork == null || connectionIdList.Count == 0)
        {
            Append("No connection. Unable send message.");
        }
        else
        {
            Log("Sending message: " + msg);
            byte[] msgData = Encoding.UTF8.GetBytes(msg);
            foreach(KeyValuePair<int, ConnectionId> kvp in connectionMappingDictToUsers)
            {
                Log("Sent to: " + kvp.Value.id);
                mediaNetwork.SendData(kvp.Value, msgData, 0, msgData.Length, reliable);
            }
            /*
            for(int i = 0; i < connectionIdList.Count; i++)
            {
                Log("Sent to: " + connectionIdList[i].id);
                mediaNetwork.SendData(connectionIdList[i], msgData, 0, msgData.Length, reliable);
            }*/
            Append(msg);
            
        }
    }
    public void SendMessageString(string msg)
    {
        bool reliable = true;
        if(msg[0] == (char)'/')
        {
            Debug.Log("Chat command activated");
            ChatCommand(messageField.text);
            return;
        }
        if (mediaNetwork == null || connectionIdList.Count == 0)
        {
            Log("No connection. Unable send message.");
        }
        else
        {
            Log("Sending message: " + msg);
            byte[] msgData = Encoding.UTF8.GetBytes(msg);
            for(int i = 0; i < connectionIdList.Count; i++)
            {
                mediaNetwork.SendData(connectionIdList[i], msgData, 0, msgData.Length, reliable);
            }
            /*
            foreach (ConnectionId id in connectionIdList)
            {
                mediaNetwork.SendData(id, msgData, 0, msgData.Length, reliable);
            }*/
        }
    }
    public void SendMessageString(SConnectionMapping msg)
    {
        bool reliable = true;
        string json = "";
        json = JsonUtility.ToJson(msg);
        if (mediaNetwork == null || connectionIdList.Count == 0)
        {
            Log("No connection. Unable send message.");
        }
        else
        {
            Log("Sending message: " + json);
            byte[] msgData = Encoding.UTF8.GetBytes(json);
            for(int i = 0; i < connectionIdList.Count; i++)
            {
                mediaNetwork.SendData(connectionIdList[i], msgData, 0, msgData.Length, reliable);
            }
            /*
            foreach (ConnectionId id in connectionIdList)
            {
                mediaNetwork.SendData(id, msgData, 0, msgData.Length, reliable);
            }*/
        }
    }
    public void SendPM(SConnectionMapping msg)
    {
        bool reliable = true;
        string json = "";
        json = JsonUtility.ToJson(msg);
        if (mediaNetwork == null || connectionIdList.Count == 0)
        {
            Log("No connection. Unable send message.");
        }
        else
        {
            Log("Sending message: " + json);
            byte[] msgData = Encoding.UTF8.GetBytes(json);
            mediaNetwork.SendData(msg.cID, msgData, 0, msgData.Length, reliable);
            /*
            foreach (ConnectionId id in connectionIdList)
            {
                mediaNetwork.SendData(id, msgData, 0, msgData.Length, reliable);
            }*/
        }
    }
    
    public void MapConnectionsToDictAdd(int playerID, ConnectionId connectionID)
    {
        connectionMappingDict.Add(connectionID, playerID);
        connectionMappingDictToUsers[playerID] = connectionID;
        try
        {
            MapConnectionsToDictRefreshUnique();
        } catch(Exception e)
        {
            Debug.Log("Unable to refresh connections due to: " + e);
        }
        
        foreach(KeyValuePair<ConnectionId, int> kvp in connectionMappingDict)
        {
            Debug.Log("<color=blue> After Adding, Current is, Connection id: " + kvp.Key + "    Player Id: " + kvp.Value +"</color>");
        }
    }

    public void MapConnectionsToDictRemove(ConnectionId connectionID, bool forceRemove = false)
    {
        Dictionary<ConnectionId, int> tempKvp = new Dictionary<ConnectionId, int>();
        int tempPlayerID = -1;

        if(connectionMappingDict.ContainsKey(connectionID))
        {
            tempPlayerID = connectionMappingDict[connectionID];
            tempKvp[connectionID] = tempPlayerID;
            
            if(connectionMappingDictToUsers.ContainsKey(connectionMappingDict[connectionID]) && forceRemove)
            {
                connectionMappingDictToUsers.Remove(connectionMappingDict[connectionID]);
            }
                
            
            connectionMappingDict.Remove(connectionID);
        }

        try
        {
            MapConnectionsToDictRefreshUnique();
        } catch(Exception e)
        {
            Debug.Log("Unable to refresh connections due to: " + e);
        }
  
        Debug.Log("<color=orange>Removed: </color> Connection ID: " + connectionID.id + "    Player ID: " + tempPlayerID);
                
        foreach(KeyValuePair<ConnectionId, int> kvp in connectionMappingDict)
        {
            Debug.Log("<color=blue> After Removing, Current is, Connection id: " + kvp.Key + "    Player Id: " + kvp.Value +"</color>");
        }       
    }
    
    //removes any extra connections
    public void MapConnectionsToDictRefreshUnique()
    {
        foreach(KeyValuePair<ConnectionId, int> kvp in connectionMappingDict)
        {
            //Debug.Log("Refreshing: " + kvp.Key.id);
            //Debug.Log("Refreshing: " + connectionMappingDictToUsers[kvp.Value]);
            if(connectionMappingDictToUsers[kvp.Value] != kvp.Key)
            {
                DisconnectPlayerVoice(kvp.Key);
                Debug.Log("<color=yellow> Disconnecting duplicate Connection id: " + kvp.Key + "    with Player Id: " + kvp.Value +"</color>");
            }     
            
                  
        }

        if(connectionMappingDict.Count == 0)
        {
            Debug.Log("There is no longer any players in the dict");
        }

        foreach(KeyValuePair<ConnectionId, int> kvp in connectionMappingDict)
        {
            Debug.Log("<color=green> After Refreshing, Current is, Connection id: " + kvp.Key + "    Player Id: " + kvp.Value +"</color>");
        }      
    }
    #endregion MediaNetwork

    private void Log(string txt)
    {
        Debug.Log("Instance " + PhotonNetwork.LocalPlayer.NickName + ": " + txt);
    }

    public void SendButtonPressed()
    {
        string msg = PhotonNetwork.NickName + ": " + messageField.text;
        if(messageField.text[0] == (char)'/')
        {
            Debug.Log("Chat command activated");
            ChatCommand(messageField.text);
            return;
        }
        SendMsg(msg);
    }

    public IEnumerator CustomVoiceReconnect(bool forceVideoOn = false)
    {
        byte[] msgData;
        string json;
        SConnectionMapping sCon = new SConnectionMapping();
        sCon.cID = new ConnectionId(-5);
        sCon.connectionId = -5;
        sCon.forceReconnect = true;
        sCon.playerID = PhotonNetwork.LocalPlayer.ActorNumber;
        
        
        if(connectionMappingDictToUsers.Count > 0)
            connectionMappingDictToUsers.Clear();
    

        if(connectionMappingDict.Count > 0)
            connectionMappingDict.Clear();  


        yield return new WaitForSeconds(mediaNetworkConnectionTime/8);    
        UnityCallFactory.EnsureInit(OnCallFactoryReadyMediaNetwork, OnCallFactoryFailedMediaNetwork);

        yield return new WaitForSeconds(mediaNetworkConnectionTime/4);
        Debug.Log("allowed video: " + UnityCallFactory.Instance.GetDefaultVideoDevice());
        Debug.Log("allowed video bool: " + forceVideoOn);
        if(UnityCallFactory.Instance.GetDefaultVideoDevice() != "" && forceVideoOn)
        {
            Debug.Log("Allowed video");
            videoOn = true;
            forceVideoOff = false;
            userAllowPermissions.allowVideo = true; 
        } else 
        {
            Debug.Log("Unallowed video");
            videoOn = false;
            forceVideoOff = true;
            userAllowPermissions.allowVideo = false; 
        }

        mediaConfig = CreateMediaConfig();
        mediaConfigUse = mediaConfig;
        

        /*
        yield return new WaitForSeconds(mediaNetworkConnectionTime/3);
        //need a bit of wait time to start the server


        StartCoroutine(InitWebRTC(PhotonNetwork.LocalPlayer.ActorNumber));
*/
        yield return new WaitForSeconds(mediaNetworkConnectionTime/4);
        ReconnectAllVoiceID();
        
        yield return new WaitForSeconds(mediaNetworkConnectionTime/2);
        json = JsonUtility.ToJson(sCon);
        msgData = Encoding.UTF8.GetBytes(json);
        //string msg = Encoding.UTF8.GetString(buffer.Buffer, 0, buffer.ContentLength);
        //connectionMapping = JsonUtility.FromJson<SConnectionMapping>(msg);
        foreach(KeyValuePair<int, ConnectionId> kvp in connectionMappingDictToUsers)
        {
            Log("Sent reset request to: " + kvp.Value.id);
            mediaNetwork.SendData(kvp.Value, msgData, 0, msgData.Length, true);
        }
    }

    public void ChatCommand(string msg)
    {
        switch(msg)
        {
            case "/video":
                Debug.Log("Video'd");
                if(isMediaNetwork)
                {
                    MediaNetworkDispose();

                    StartCoroutine(CustomVoiceReconnect(true));
                    MapConnectionsToDictRefreshUnique();
                    
                } 
                break;
            case "/videooff":
                Debug.Log("Video off'd");
                if(isMediaNetwork)
                {
                    MediaNetworkDispose();

                    StartCoroutine(CustomVoiceReconnect());
                    MapConnectionsToDictRefreshUnique();
                } 
                
                break;
            case "/test":
                SConnectionMapping mapping;
                mapping.connectionId = 69;
                mapping.playerID = 420;
                mapping.cID = new ConnectionId(420);
                mapping.forceReconnect = false;
                SendMessageString(mapping);
                break;
            default:
                break;
        }  
    }
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
    
    
    
    

