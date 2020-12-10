using System.Collections.Generic;
using UnityEngine;
using Photon.Chat;
using ExitGames.Client.Photon;
using Photon.Pun;
using UnityEngine.UI;
using System;

public class ChatMasenger : MonoBehaviour, IChatClientListener
{
    public static ChatMasenger Instance;

    ChatClient chatClient;

    [Header("Texts")]

    [SerializeField] string UserName;
    public InputField messageToSend;
    public InputField ReciverPlayer;
    public Text NameFeild;
    public Text MainChatUnOpen;

    [Header("Counters")]

    int MainChatCounter = 0;

    [Header("Panels")]

    public GameObject ChatPanel;
    public GameObject PrivateChatPanel;
    public GameObject MsgScreen;
    public GameObject PlayerMsgScreen;
    public Image MainChatBtnImg;

    [Header("ChannelName")]

    public string ChanelName = "Room";

    [Header("Prefabs")]

    public Text PlayerMsgPrefab;
    public GameObject PlayerPrefab;
    public Transform BtnContent;
    public GameObject ButonContent;

    [Header("Lists")]

    public List<GameObject> PlayersBtnList = new List<GameObject>();
    public List<string> PlayersNamesList = new List<string>();

    [HideInInspector] public GameObject currentScreen;

    [Header("Audio")]

    public AudioSource MsgNotification;

    public void Awake()
    {
        if(Instance==null)
            Instance = this;
    }
    void Start()
    {
        chatClient = new ChatClient(this);

        Connect(PlayerPrefs.GetString("Name"));
    }

    public void DebugReturn(DebugLevel level, string message)
    {
        //throw new System.NotImplementedException();
    }

    public void Connect(string UserName)
    {
        this.UserName = UserName;
        this.chatClient = new ChatClient(this);
        chatClient.ChatRegion = "EU";
        this.chatClient.Connect(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat, "1.0", new Photon.Chat.AuthenticationValues(this.UserName));
    }
    
    public void OnChatStateChange(ChatState state)
    {
        //throw new System.NotImplementedException();
    }

    public void OnConnected()
    {
        chatClient.Subscribe(ChanelName, creationOptions: new ChannelCreationOptions { PublishSubscribers = true });
    }

    public void OnDisconnected()
    {
        //throw new System.NotImplementedException();
    }

    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        //throw new System.NotImplementedException();
    }

    public void OnClickSend()
    {
        if (this.messageToSend.text != "")
        {
            chatClient.SendPrivateMessage(ReciverPlayer.text, messageToSend.text);
            this.messageToSend.text = "";
            messageToSend.ActivateInputField();
        }
    }

    public void SendDirectMsg(string name)
    {
        ReciverPlayer.text = name;
        ChatPanel.SetActive(true);
        PrivateChatPanel.SetActive(true);
        currentScreen = MsgScreen.transform.Find(name).gameObject;
        GetChildWithName(currentScreen, name);
        messageToSend.Select();
    }

    public void OnPrivateMessage(string sender, object message, string channelName)
    {
        String temp=("\n " + sender+": ");

        Text txt = Instantiate(PlayerMsgPrefab);

        if (sender == this.UserName)
        {
            GameObject PlayerToSendMsg = currentScreen;
            txt.transform.SetParent(PlayerToSendMsg.gameObject.transform.GetChild(0).GetChild(0));
        }
        else if(sender != this.UserName)
        {
            GameObject PlayerWhoSendMsg = MsgScreen.transform.Find(sender).gameObject;
            txt.transform.SetParent(PlayerWhoSendMsg.gameObject.transform.GetChild(0).GetChild(0));
        }

        txt.text = temp + message;

        if(txt.text.Length>40)
        {
            int a = (txt.text.Length / 40) + 1;
            txt.GetComponent<RectTransform>().sizeDelta = new Vector2(1200, 57*a);
        }

        for (int i = 0; i < PlayersNamesList.Count; i++)
        {
            if (sender == PlayersNamesList[i])
            {
                if (sender == PlayersNamesList[i])
                {
                    if (currentScreen == null)
                    {
                        currentScreen = MsgScreen.transform.GetChild(0).gameObject;
                    }
                    if (sender == currentScreen.name && currentScreen.activeInHierarchy)
                    {
                        Debug.Log("Don't Send Notification");
                    }
                    else
                    {
                        int num = 0;
                        num = int.Parse(PlayersBtnList[i].GetComponent<PlayerBtnClicked>().PlayerBtnCountText.text);
                        num++;
                        PlayersBtnList[i].GetComponent<PlayerBtnClicked>().PlayerBtnCountText.text = num.ToString();
                        if (num > 0)
                        {
                            PlayersBtnList[i].GetComponent<PlayerBtnClicked>().PlayerBtnCountImage.gameObject.SetActive(true);
                        }
                        num = 0;
                    }
                }
            }
        }

        if (!ChatPanel.activeInHierarchy)
        {
            CountFromList();
            MsgNotification.Play();
        }

        txt.transform.localScale = Vector3.one;
    }

    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
        //throw new System.NotImplementedException();
    }

    public void OnSubscribed(string[] channels, bool[] results)
    {
        /// calls when connected to chat channel
        Debug.Log("Connected to chat channel");

        PlayerPrefab.SetActive(false);

        ChatChannel temp = null;

        bool found = this.chatClient.TryGetChannel(ChanelName, out temp);

        if (found)
        {
            var users = temp.Subscribers;

            foreach (var item in users)
            {
                if (UserName != item)
                {
                    GameObject Playerbtn = Instantiate(this.PlayerPrefab);
                    Playerbtn.transform.SetParent(BtnContent, false);
                    Playerbtn.GetComponent<PlayerBtnClicked>().playerName = item.ToString();
                    Playerbtn.GetComponentInChildren<Text>().text = item.ToString();
                    PlayersBtnList.Add(Playerbtn);
                    Playerbtn.SetActive(true);
                    GameObject PrivateMsgScreen = (GameObject)Instantiate(this.PlayerMsgScreen);
                    PrivateMsgScreen.transform.SetParent(MsgScreen.transform, false);
                    PrivateMsgScreen.name = item.ToString();
                    PrivateMsgScreen.SetActive(true);
                    PlayersNamesList.Add(item.ToString());
                }
            }
        }
    }

    public void OnMainChatBtnClick(bool toggle)
    {
        if (toggle)
        {
            MainChatBtnImg.gameObject.SetActive(false);
            ChatPanel.SetActive(true);
        }
        else
        {
            ChatPanel.SetActive(false);
            PrivateChatPanel.SetActive(false);
            CountFromList();
        }
    }

    public void CountFromList()
    {
        MainChatCounter = 0;
        for (int i = 0; i < PlayersNamesList.Count; i++)
        {
            int Tnum = 0;
            Tnum = int.Parse(PlayersBtnList[i].GetComponent<PlayerBtnClicked>().PlayerBtnCountText.text);
            MainChatCounter += Tnum;
            Tnum = 0;
        }
        if (MainChatCounter > 0)
        {
            MainChatBtnImg.gameObject.SetActive(true);
        }
        else
        {
            MainChatBtnImg.gameObject.SetActive(false);
        }

        MainChatUnOpen.text = MainChatCounter.ToString();
    }

    public void OnUnsubscribed(string[] channels)
    {
        //throw new System.NotImplementedException();
    }

    public void OnUserSubscribed(string channel, string user)
    {
        /// calls when new player enters the chat

        ChatChannel chatChannel = null;
        bool found = this.chatClient.TryGetChannel(ChanelName, out chatChannel);
        if (found)
        {
            GameObject Playerbtn = (GameObject)Instantiate(this.PlayerPrefab);
            Playerbtn.transform.SetParent(BtnContent, false);
            Playerbtn.gameObject.transform.GetChild(0).GetComponent<Text>().text = user.ToString();
            Playerbtn.SetActive(true);
            PlayersBtnList.Add(Playerbtn);
            GameObject PrivateMsgScreen = (GameObject)Instantiate(this.PlayerMsgScreen);
            PrivateMsgScreen.transform.SetParent(MsgScreen.transform, false);
            PrivateMsgScreen.name = user.ToString();
            PrivateMsgScreen.SetActive(true);
            PlayersNamesList.Add(user.ToString());
        }
    }

    void Update()
    {
        if (chatClient != null)
        {
            chatClient.Service();
        }
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            OnClickSend();
        }
    }

    public void OnUserUnsubscribed(string channel, string user)
    {
        Debug.Log(user + " Player Unsubscribed ");
        for (int i = 0; i < PlayersNamesList.Count; i++)
        {
            if (user == PlayersNamesList[i])
            {
                Debug.Log(PlayersNamesList[i]);
                PlayersNamesList.RemoveAt(i);
                GameObject BtnToDestroy = PlayersBtnList[i];
                PlayersBtnList.RemoveAt(i);
                Destroy(BtnToDestroy);
                Destroy(MsgScreen.transform.Find(user).gameObject);
            }
        }
    }

    public void GetChildWithName(GameObject currentPlayerScreen, string playerName)
    {
        currentPlayerScreen.transform.SetAsLastSibling();
    }
}
