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
    
    public const int MAX_CODE_LENGTH = 256;

    protected ICall iCall;                                              // The original API that we were using for the conference call. Not used anymore because of connection issues.
    public ConnectionId idSingleVideo;    
    private IMediaNetwork mediaNetwork = null;                          // Current API for audio/video networking, recommended by the author who made the plugin
    protected MediaConfig mediaConfigUse;
    protected MediaConfig mediaConfig;
    protected IEnumerator autoRejoinCoroutine = null;
    
    protected ConnectionId remoteUserId = ConnectionId.INVALID;
    SConnectionMapping thisConnectionMap = new SConnectionMapping();

    public string uSignalingUrl = "ws://rtc-ignite-voice.herokuapp.com/conferenceapp";
    public string uSecureSignalingUrl = "wss://rtc-ignite-voice.herokuapp.com/conferenceapp";
    public bool uForceSecureSignaling = false;
    public string uIceServer = "turn:numb.viagenie.ca";
    public string uIceServerUser = "webrtc@live.com";
    public string uIceServerPassword = "muazkh";
    public string uIceServer2 = "stun:stun.l.google.com:19302";
    public string uIceServer3 = "stun:stun.stunprotocol.org:3478";
    public string voiceID = "";
    private string localAddress = null;
    protected string targetAddress = null;

    
    public List<int> idWithVideos = new List<int>();
    public List<int> connectedVoiceID = new List<int>();
    private List<ConnectionId> connectionIdList = new List<ConnectionId>();

    
    private Dictionary<ConnectionId, VideoData> mVideoUiElements = new Dictionary<ConnectionId, VideoData>();       // Current video display game objects in the scene for webcam
    public Dictionary<ConnectionId, int> connectionMappingDict = new Dictionary<ConnectionId, int>();       
    public Dictionary<int, ConnectionId> connectionMappingDictToUsers = new Dictionary<int, ConnectionId>();
    public Dictionary<int, ConnectionId> connectionMappingDictToUsersBackUp = new Dictionary<int, ConnectionId>();
    
    public int videoCount = 0;

    protected float rejoinTime = 0.2f;
    public float noVidWaitTime = 4.0f;
    public float noTextureTimer;
    private float mediaNetworkConnectionTime = 4.0f;

    public bool userAllowAudio = true;
    public bool audioOn = true;
    public bool videoOn = false;
    public bool userAllowVideo = false;                     
    public bool startedVoiceServer = false;
    public bool lockChatVisibility = false;
    
    public bool isConference = true;
    public bool reconnectingVoiceCoroutine = false;
    public bool hasFrame;                               //For webcam display if there are frames incoming from connection
    private bool forceVideoOff = true;
    private bool isICall = false;                       //Disabled for now due to connection bugs
    private bool isMediaNetwork = true;                 //Sets to use IMediaNetwork API
    public bool firstJoinedVid = false;
    protected bool iCallActive = false;
    protected bool localFrameEvents = true;             //For the host who is showing the webcam to allow them to display themselves
    protected bool autoRejoin = true;

    public GameObject receiveTxt;
    public GameObject[] videoImage;
    public ChatText textOutput;    
    public InputField messageField;    
    public Scrollbar scrollbar;
    public AudioCallUI audioCallUI;    
    public UserPermissionCommunication userAllowPermissions;            // Game Object that contains the permission settings of user
    public Texture2D noVidTexture;        
    private Texture2D videoTextureBuffer;    
    

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
        //default value for initialized connection id
        thisConnectionMap.connectionId = -2;
    }
    #endif
    
    void Start()
    {
        scrollbar.value = 0;
        mediaConfig = CreateMediaConfig();
        mediaConfigUse = mediaConfig; 
        userAllowPermissions = UserPermissionCommunication.instance;

        //in case we have any UI we want to add in.
        if(GetComponent<AudioCallUI>()) 
        {
            audioCallUI = GetComponent<AudioCallUI>();
        }
    
        StartCoroutine(UpdateNoTexture());
        StartCoroutine(AutoReconnectVoiceId(autoRejoin));
    }

    void Update()
    {
        if (mediaNetwork == null)
            return;
        
        //This is the event handler via polling.
        //This needs to be called or the memory will fill up with unhanded events!
        NetworkEvent evt;

        mediaNetwork.Update();

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

    #region MediaNetwork
    protected virtual void OnCallFactoryReadyMediaNetwork()
    {
        UnityCallFactory.Instance.RequestLogLevel(UnityCallFactory.LogLevel.Info);
        if(PhotonNetwork.LocalPlayer.ActorNumber != -1)
        {
            StartCoroutine(InitWebRTC(PhotonNetwork.LocalPlayer.ActorNumber));
        }
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

            //Usually the Turn servers
            if (string.IsNullOrEmpty(uIceServer) == false)
            {
                netConfig.IceServers.Add(new IceServer(uIceServer, uIceServerUser, uIceServerPassword));
            }
            if (string.IsNullOrEmpty(uIceServer2) == false)
            {
                netConfig.IceServers.Add(new IceServer(uIceServer2));
            }
            // The 3rd server usually isn't necessary
            if (string.IsNullOrEmpty(uIceServer3) == false)
            {
                netConfig.IceServers.Add(new IceServer(uIceServer3));
            }          

            if (Application.platform == RuntimePlatform.WebGLPlayer || uForceSecureSignaling)
            {
                netConfig.SignalingUrl = uSecureSignalingUrl;
            }
            else
            {
                netConfig.SignalingUrl = uSignalingUrl;
            }

            mediaNetwork = UnityCallFactory.Instance.CreateMediaNetwork(netConfig);
            
            //Debug.Log("Example vid dev is: " + UnityCallFactory.Instance.GetDefaultVideoDevice());
            if(userAllowPermissions)
            {
                if(UnityCallFactory.Instance.GetDefaultVideoDevice() != "" && userAllowPermissions.allowVideo)
                {
                    videoOn = true;   
                }
            }

            mediaConfig.Audio = audioOn;

            if(forceVideoOff)
            {
                mediaConfig.Video = false;
                videoOn = false;
            } else
            {
                mediaConfig.Video = videoOn;
            }

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
            //below code is buggy, won't reconnect correctly.
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

    /// <summary>
    /// Sets the default IMediaNetwork configuration.
    /// </summary>
    /// <returns>
    /// MediaConfig
    /// </returns>
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

        if(userAllowPermissions)
        {
            if(UnityCallFactory.Instance.GetDefaultVideoDevice() != "" && userAllowPermissions.allowVideo)
            {
                videoOn = true;   
            }
        }
        
        mediaConfig.Audio = audioOn;
        if(forceVideoOff)
        {
            mediaConfig.Video = false;
        } else
        {
            mediaConfig.Video = videoOn;
        }
        
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

    /// <summary>
    /// Force reconnection to all current players
    /// </summary>
    public void ReconnectAllVoiceID()
    {
        Photon.Realtime.Player[] playerList = PhotonNetwork.PlayerListOthers;
        for(int i = 0; i < playerList.Length; i++)
        {
            ConnectToPlayerVoiceId(playerList[i].ActorNumber);
        }
    }

    /// <summary>
    /// Temporary measure to keep checking for connection because it's still quite buggy
    /// </summary>
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

    public void ConnectToPlayerVoiceId(int playerActorID)
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
    
    public void DisconnectPlayerVoiceId(ConnectionId cID)
    {       
        mediaNetwork.Disconnect(cID);
    }
    
    /// <summary>
    /// Reconnect ONLY in the event that a connection has failed.
    /// </summary>
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

    /// <summary>
    /// Resets the entire WebRTC connection modules and parameters.
    /// </summary>
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
                DisconnectPlayerVoiceId(kvp.Key);
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

    /// <summary>
    /// Called from buttons
    /// </summary>
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
    
    /// <summary>
    /// Reconnects WebRTC to other clients to adjust video settings
    /// </summary>
    /// <param name="forceVideoOn">The bool parameter is only for setting the MediaConfig video option to True from all other clients</param>
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
        
        yield return new WaitForSeconds(mediaNetworkConnectionTime/4);
        ReconnectAllVoiceID();
        
        yield return new WaitForSeconds(mediaNetworkConnectionTime/2);
        json = JsonUtility.ToJson(sCon);
        msgData = Encoding.UTF8.GetBytes(json);

        foreach(KeyValuePair<int, ConnectionId> kvp in connectionMappingDictToUsers)
        {
            Log("Sent reset request to: " + kvp.Value.id);
            mediaNetwork.SendData(kvp.Value, msgData, 0, msgData.Length, true);
        }
    }

    /// <summary>
    /// Command-line from text chat
    /// <para>/video - Enables video</para>
    /// <para>/videooff - Disables video</para>
    /// </summary>
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
        #if UNITY_WEBGL
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
        #endif
    }

    /// <summary>
    /// Adds a new message to the message view
    /// </summary>
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

    /// <summary>
    /// Makes sure the chat stays at the bottom of the text view
    /// </summary>
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
    
    /// <summary>
    /// Sets the volume of the user, either 0 for silence, or 1 for full volume
    /// </summary>
    /// <param name="remoteUser">The targetted connection.</param>
    /// <param name="val">The targetted connection's volume adjustment.</param>
    public void ButtonVolumeChannel(ConnectionId remoteUser, float val)
    {
        if(isMediaNetwork)
        {
            mediaNetwork.SetVolume(val, remoteUser);
        } else if(isICall)
        {
            iCall.SetVolume(val, remoteUser);
        }        
    }

    public ConnectionId GetConnectionId()
    {
        return remoteUserId;
    }

    /// <summary>
    /// Used for pseudo proximity voice with ICall API
    /// </summary>
    public void SetVolume(float volume, int user)
    {
        ConnectionId tempId;
        tempId = remoteUserId;
        tempId.id = (short)PhotonNetwork.LocalPlayer.ActorNumber;
        
        //iCall.SetVolume(volume, tempId);
    }

    private void Log(string txt)
    {
        Debug.Log("Instance " + PhotonNetwork.LocalPlayer.NickName + ": " + txt);
    }
}
    
    
    
    

