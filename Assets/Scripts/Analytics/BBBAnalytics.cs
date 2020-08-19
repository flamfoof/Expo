using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class BBBAnalytics : IgniteAnalytics, IPunObservable
{
    public IgniteGameManager gameManager;
    public static BBBAnalytics instance;
    public int attendees = 0;
    public int clicks = 0;
    public float avgSessionTime = 0;
    List<string> sessionNameList;
    List<float> sessionTimeList;
    List<int> playersLogged;

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
            instance = this.gameObject.GetComponent<BBBAnalytics>();            
        }
        sessionNameList = new List<string>();
        sessionTimeList = new List<float>();
        if(!gameManager)
            gameManager = IgniteGameManager.IgniteInstance;

        InvokeRepeating("UpdateAvgSessionTime", 1.0f, 1.0f);
        InvokeRepeating("AnalyticsAvgTimeUpdate", 1.0f, 5.0f);
    }

    void FixedUpdate()
    {
        //UpdateAttendeesCount();
        UpdateAllTexts();
    }

    public override void ClickedStats(string url)
    {
        photonView.RPC("TriggeredEvent", RpcTarget.All);
        Invoke("AnalyticsUpdateClicks", 0.5f);
    }

    void AnalyticsUpdateClicks(string url)
    {
        if(AnalyticsController.Instance)
        {
            AnalyticsController.Instance.WebsiteClick(url);
        }
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
        //Debug.Log("Updating sesion time");
        foreach(PhotonView pv in GameObject.FindObjectsOfType(typeof(PhotonView)))
        {
            //Debug.Log("Starting this");
            //Debug.Log(pv.name);
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
                    //Debug.Log("Added new player: " + pv.Owner.NickName + " to the list.");
                }
            } 
        }
        if(attendees != 0)
        {
            avgSessionTime = totalSessionTime / attendees;
        } else {
            avgSessionTime = 0.0f;
        }


        /* Trying to get info from network, not working
        for(int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {   
            string playerName = PhotonNetwork.PlayerList[i].ActorNumber.ToString();
            if(sessionNameList.Contains(playerName))
            {
                int index = sessionNameList.IndexOf(playerName);
                sessionTimeList[index] = (float)PhotonNetwork.PlayerList[i].CustomProperties["LoginTime"];
                
            } else {
                sessionNameList.Add(PhotonNetwork.PlayerList[i].NickName);
                sessionTimeList.Add((float)PhotonNetwork.PlayerList[i].CustomProperties["LoginTime"]);
            }
        }*/
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
        attendeesText.text = "Attendees: " + attendees;
        AnalyticsController.Instance.AttendesNumber(attendees);
        sessionText.text = "Avg. Session Time: " + sessionTempText;
        AnalyticsController.Instance.AverageTimeSpent(avgSessionTime);
        clicksText.text = "Web Clicks: " + clicks;
    }

    //may not be necessary
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {
            stream.SendNext(clicks);  
        } else {
            clicks = (int) stream.ReceiveNext();
        }
                
    }
}
