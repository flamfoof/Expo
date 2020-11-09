using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System;
using UnityEngine.Assertions;
using System.Linq;

public class BBBAnalytics : MonoBehaviour
{
    public static BBBAnalytics instance;

    public IgniteGameManager gameManager;
    public int attendees = 0;
    public int clicks = 0;
    public float avgSessionTime = 0;
    public string sessionStartTime;
    List<string> sessionNameList;
    List<float> sessionTimeList;
    List<int> playersLogged;

    private APIHandler apiHandler;

    Dictionary<string, string> clickedVideos = new Dictionary<string, string>();
    Dictionary<string, string> clickedWebLinks = new Dictionary<string, string>();
    Dictionary<string, string> emojiUsed = new Dictionary<string, string>();
    List<string> chatLog = new List<string>();


    public Text attendeesText;
    public Text sessionText;
    public Text clicksText;

    private void Awake()
    {

        if (instance != null)
        {
            DestroyImmediate(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        apiHandler = GetComponent<APIHandler>();
        Assert.IsNotNull(apiHandler);

        sessionStartTime = GetCurrentTime();
    }

    private void Start() 
    {
        sessionNameList = new List<string>();
        sessionTimeList = new List<float>();
        if(!gameManager)
            gameManager = IgniteGameManager.IgniteInstance;

        InvokeRepeating("UpdateAvgSessionTime", 1.0f, 1.0f);
        //InvokeRepeating("AnalyticsAvgTimeUpdate", 1.0f, 5.0f);
    }

    public string GetCurrentTime()
    {
        string[] data = DateTime.UtcNow.ToString().Split(' ');
        return data[1] + data[2];
    }

    public string GetCurrentDate()
    {
        string[] data = DateTime.UtcNow.ToString().Split(' ');
        return data[0];
    }


    private void OnEnable()
    {
        Timer.sendAnalytics += DispatchAnalytics;
    }

    public void OnDisable()
    {
        Timer.sendAnalytics -= DispatchAnalytics;
    }

    [ContextMenu("DispatchAnalytics")]
    async void DispatchAnalytics()
    {
        if (clickedVideos.Count > 0)
        {
            for (int i = 0; i < clickedVideos.Count; i++)
            {
                //Debug.Log("Looping dictionary clickedVideos " + PlayerPrefs.GetString("Name") + "Data " + clickedVideos.ElementAt(i).Key
                //    + "Date " + clickedVideos.ElementAt(i).Value);
                await apiHandler.Actions(PlayerPrefs.GetString("Name"), "video_click", clickedVideos.ElementAt(i).Key, clickedVideos.ElementAt(i).Value);
            }
        }

        if (clickedWebLinks.Count > 0)
        {
            for (int i = 0; i < clickedWebLinks.Count; i++)
            {
                //Debug.Log("Looping dictionary clickedWebLinks " + PlayerPrefs.GetString("Name") + "Data " + clickedWebLinks.ElementAt(i).Key
                //    + "Date " + clickedWebLinks.ElementAt(i).Value);
                await apiHandler.Actions(PlayerPrefs.GetString("Name"), "link_click", clickedWebLinks.ElementAt(i).Key, clickedWebLinks.ElementAt(i).Value);
            }
        }

        if (emojiUsed.Count > 0)
        {
            for (int i = 0; i < emojiUsed.Count; i++)
            {
                //Debug.Log("Looping dictionary emojiUsed " + PlayerPrefs.GetString("Name") + "Data " + emojiUsed.ElementAt(i).Key
                //    + "Date " + emojiUsed.ElementAt(i).Value);
                await apiHandler.Actions(PlayerPrefs.GetString("Name"), "emoji_used", emojiUsed.ElementAt(i).Key, emojiUsed.ElementAt(i).Value);
            }
        }

        ResetDictionaries();
    }

    void ResetDictionaries()
    {
        clickedVideos.Clear();
        clickedWebLinks.Clear();
        emojiUsed.Clear();
    }

    void FixedUpdate()
    {
        UpdateAllTexts();
    }

    public async void EndSession(Action callback)
    {
        await apiHandler.Access(PlayerPrefs.GetString("Name"), GetCurrentDate() , sessionStartTime , GetCurrentTime());

        callback?.Invoke();
    }


    private async void OnApplicationQuit()
    {
        await apiHandler.Access(PlayerPrefs.GetString("Name"), GetCurrentDate(), sessionStartTime, GetCurrentTime());
    }

    public void ClickedVideo(string name)
    {
        clickedVideos.Add(name, DateTime.UtcNow.ToString());
    }

    public void EmojiUsed(string name)
    {
        emojiUsed.Add(name, DateTime.UtcNow.ToString());
    }
    
    public void ClickedWeb(string url)
    {
        clickedWebLinks.Add(url, DateTime.UtcNow.ToString());
    }

    public void AverageSessionLength()
    {
        Debug.Log(avgSessionTime);
    }

    public void UpdateChatLog(string log)
    {
        chatLog.Add(log);
    }


    void AnalyticsUpdateClicks(string url)
    {
        //if(AnalyticsController.Instance)
        //{
        //    AnalyticsController.Instance.WebsiteClick(url);
        //}


    }

    [PunRPC]
    void TriggeredEvent(string url)
    {
        clicks++;
    }

    void UpdateAvgSessionTime()
    {
        float totalSessionTime = 0.0f;
        int totalPlayers = 0;
        foreach(PhotonView pv in GameObject.FindObjectsOfType(typeof(PhotonView)))
        {

            if(pv.gameObject)
            {
                //Debug.Log("still alive");
            }

            if(pv.GetComponent<UserActions>())
            {
                if(sessionNameList.Contains(pv.Owner.NickName))
                {
                    int index = sessionNameList.IndexOf(pv.Owner.NickName);
                    sessionTimeList[index] = pv.GetComponent<UserActions>().SessionTimer;
                    totalSessionTime += sessionTimeList[index];
                    totalPlayers++;
                    //Debug.Log("set time of player: " + sessionNameList[index] + " to timer: " + sessionTimeList[index]);
                } else {
                    sessionNameList.Add(pv.Owner.NickName);
                    sessionTimeList.Add(pv.GetComponent<UserActions>().SessionTimer);
                    totalSessionTime += pv.GetComponent<UserActions>().SessionTimer;
                    totalPlayers++;
                }
            } 
        }
        if(attendees != 0)
        {
            avgSessionTime = totalSessionTime / attendees;
        } else {
            avgSessionTime = 0.0f;
        }
    }

    void AnalyticsAvgTimeUpdate()
    {                    
        //invoked
        if(AnalyticsController.Instance)
        {
            AnalyticsController.Instance.AverageTimeSpent(avgSessionTime);
        }
    }

    

    public void UpdateAttendeesCount(int playerID)
    {
        /*
        if(gameManager)
        {
            if(PhotonNetwork.IsMasterClient)
            {
                if(attendees != gameManager.totalUniquePlayers)
                {
                    //attendees++;
                    Debug.Log("Attendees: " + attendees + " game manager attendee: " + gameManager.totalUniquePlayers);
                    photonView.RPC("AddPlayerCount", RpcTarget.AllBufferedViaServer, playerID);     
                }
            }
        }*/
        Debug.Log("attendee count: " + attendees);
        attendees = playerID;
    }

    [PunRPC]
    void AddPlayerCount(int playerID)
    {
        Debug.Log("Adding attendees: " + playerID);
        if(!gameManager.uniquePlayersLogged.Contains(playerID))
        {
            gameManager.uniquePlayersLogged.Add(playerID);  
        }
        attendees++;
    }

    void UpdateAllTexts()
    {
        string sessionTempText = "";
        if(avgSessionTime > 60)
        {
            sessionTempText = Mathf.FloorToInt(avgSessionTime/60) + " m " + System.Math.Round(avgSessionTime % 60, 2) + "s";
        } else {
            sessionTempText = System.Math.Round(avgSessionTime, 2) + "s";
        }
        //attendeesText.text = "Attendees: " + attendees;
        //AnalyticsController.Instance.AttendesNumber(attendees);
        //sessionText.text = "Avg. Session Time: " + sessionTempText;
        //AnalyticsController.Instance.AverageTimeSpent(avgSessionTime);
        //clicksText.text = "Web Clicks: " + clicks;
    }
}
