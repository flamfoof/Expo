using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class BBBAnalytics : IgniteAnalytics, IPunObservable
{
    public IgniteGameManager gameManager;
    public int attendees = 0;
    public int clicks = 0;
    public float avgSessionTime = 0;
    List<string> sessionNameList;
    List<float> sessionTimeList;

    public Text attendeesText;
    public Text sessionText;
    public Text clicksText;
    
    private void Start() 
    {
        sessionNameList = new List<string>();
        sessionTimeList = new List<float>();
        if(!gameManager)
            gameManager = IgniteGameManager.IgniteInstance;

        InvokeRepeating("UpdateAvgSessionTime", 1.0f, 1.0f);
        
    }

    void FixedUpdate()
    {
        UpdateAttendeesCount();
        UpdateAllTexts();
    }

    public override void ClickedStats()
    {
        photonView.RPC("TriggeredEvent", RpcTarget.All);
    }

    [PunRPC]
    void TriggeredEvent()
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
            if(pv.GetComponent<UserActions>())
            {
                if(sessionNameList.Contains(pv.Owner.NickName))
                {
                    int index = sessionNameList.IndexOf(pv.Owner.NickName);
                    sessionTimeList[index] = pv.GetComponent<UserActions>().loginTimer;
                    totalSessionTime += sessionTimeList[index];
                    totalPlayers++;
                    //Debug.Log("set time of player: " + sessionNameList[index] + " to timer: " + sessionTimeList[index]);
                } else {
                    sessionNameList.Add(pv.Owner.NickName);
                    sessionTimeList.Add(pv.GetComponent<UserActions>().loginTimer);
                    totalSessionTime += pv.GetComponent<UserActions>().loginTimer;
                    totalPlayers++;
                    //Debug.Log("Added new player: " + pv.Owner.NickName + " to the list.");
                }
            } 
        }

        avgSessionTime = totalSessionTime / totalPlayers;


        /*
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

    void UpdateAttendeesCount()
    {
        if(gameManager)
            attendees = gameManager.totalUniquePlayers;
    }

    void UpdateAllTexts()
    {
        attendeesText.text = "Attendees: " + attendees;
        sessionText.text = "Avg. Session TIme: " + avgSessionTime;
        clicksText.text = "Baby Seat Clicks: " + clicks;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {
            stream.SendNext(attendees);
            stream.SendNext(avgSessionTime);
            stream.SendNext(clicks);  
        } else {
            attendees = (int)stream.ReceiveNext();
            avgSessionTime = (float)stream.ReceiveNext();
            clicks = (int) stream.ReceiveNext();
        }
                
    }
}
