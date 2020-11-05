using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System;

public class BBBAnalytics : IgniteAnalytics
{
    public IgniteGameManager gameManager;
    public static BBBAnalytics instance;
    public int attendees = 0;
    public int clicks = 0;
    public float avgSessionTime = 0;
    List<string> sessionNameList;
    List<float> sessionTimeList;
    List<int> playersLogged;

    Dictionary<string, int> clickedVideos = new Dictionary<string, int>();
    Dictionary<string, int> clickedWebLinks = new Dictionary<string, int>();
    Dictionary<string, int> emojiUsed = new Dictionary<string, int>();
    List<string> chatLog = new List<string>();


    public Text attendeesText;
    public Text sessionText;
    public Text clicksText;
    
    private void Start() 
    {
        if(instance != null)
        {
            Destroy(this.gameObject);
        } else 
        {
            instance = this;            
        }

        sessionNameList = new List<string>();
        sessionTimeList = new List<float>();
        if(!gameManager)
            gameManager = IgniteGameManager.IgniteInstance;

        InvokeRepeating("UpdateAvgSessionTime", 1.0f, 1.0f);
        //InvokeRepeating("AnalyticsAvgTimeUpdate", 1.0f, 5.0f);
    }


    private void OnEnable()
    {
        Timer.sendAnalytics += DispatchAnalytics;
    }

    public void OnDisable()
    {
        Timer.sendAnalytics -= DispatchAnalytics;
    }

    private void DispatchAnalytics()
    {
        Debug.LogError("DispatchAnalytics");
    }

    void FixedUpdate()
    {
        UpdateAllTexts();
    }

    public override void ClickedVideo(string name)
    {
        if (clickedVideos.ContainsKey(name))
        {
            clickedVideos[name] += 1;

            Debug.Log("Times Clicked " + clickedVideos[name]);
        }
        else
        {
            clickedVideos.Add(name, 1);
            Debug.Log("Added new video");
        }
    }

    public override void EmojiUsed(string name)
    {
        if (emojiUsed.ContainsKey(name))
        {
            emojiUsed[name] += 1;

            Debug.Log("Times Clicked " + emojiUsed[name]);
        }
        else
        {
            emojiUsed.Add(name, 1);
            Debug.Log("Added new Emoji");
        }
    }
    
    public override void ClickedWeb(string url)
    {
        if (clickedWebLinks.ContainsKey(url))
        {
            clickedWebLinks[url] += 1;

            Debug.Log("Times Clicked " + clickedWebLinks[url]);
        }
        else
        {
            clickedWebLinks.Add(name, 1);
            Debug.Log("Added new link");
        }
    }

    public override void AverageSessionLength()
    {
        Debug.Log(avgSessionTime);
    }

    public override void UpdateChatLog(string log)
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
