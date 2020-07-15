using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class YTVidDistancePlay : MonoBehaviour
{
    public List<GameObject> playerList;
    public GameObject player;
    public float distanceThreshold = 6.0f;
    [Tooltip("The minimum distance away from video that")]
    public float distanceMinimumAudioDist = 4.0f;
    public float distanceToFocusedVid = 0.0f;
    public bool firstFocused = false;
    public bool isFocusedOnVid = false;
    public YoutubePlayer currentFocused;
    public YoutubePlayer prevFocused;
    int count = 0;

    public List<YoutubePlayer> listYT;

    private void Start() {        
        YoutubePlayer[] listVidPlayers = GameObject.FindObjectsOfType<YoutubePlayer>();
        foreach(YoutubePlayer vid in listVidPlayers)
        {
            if(!vid.GetComponent<LocalVideoPlay>().isLocal)
                listYT.Add(vid);
        }
        Invoke("FindPlayer", 1.0f);
//        Debug.Log("started youtube videos");
        #if UNITY_WEBGL
        //Calls an update every 0.5 seconds to this function
        InvokeRepeating("CheckPlayerDistance", 2.0f, 0.8f);

        #endif
    }

    #if UNITY_WEBGL
    private void Update() {
        if(currentFocused)
        {
            float distance;
            float minAudioDist = currentFocused.gameObject.GetComponent<LocalVideoPlay>().minDistVolume;
            float maxAudioDist = currentFocused.gameObject.GetComponent<LocalVideoPlay>().maxDistVolume;
            
            distanceToFocusedVid = Vector3.Distance(currentFocused.transform.position, player.transform.position);
            distance = maxAudioDist - distanceToFocusedVid - minAudioDist;

            Debug.Log((distance));

            if(distance < 0.0f)
                distance = 0.0f;

            if(distance > maxAudioDist)
                distance = maxAudioDist;

            currentFocused.videoPlayer.SetDirectAudioVolume(0, (distance / (maxAudioDist - minAudioDist)));
        }
    }
    #endif

    void FindPlayer()
    {
        foreach(UserActions players in GameObject.FindObjectsOfType(typeof(UserActions)))
        {
            if(players.GetComponent<PhotonView>().IsMine)
            {
                player = players.gameObject;
            }
        }
    }

    #if !UNITY_WEBGL
    private void Awake() {
        GetComponent<YoutubePlayer>().enabled = true;
    }

    #endif

    #if UNITY_WEBGL

    void CheckPlayerDistance()
    {
        for(int i = 0; i < listYT.Count; i++)
        {
            float currentDistance = Vector3.Distance(listYT[i].transform.position, player.transform.position);
            //Debug.Log("Distance: " + listYT[i].name + Vector3.Distance(listYT[i].transform.position, player.transform.position));
            if(!currentFocused)
            {
                
                if(currentDistance < distanceThreshold && !firstFocused)
                {
                    for(int j = 0; j < listYT.Count; j++)
                    {
                        firstFocused = true;
                        prevFocused = listYT[i].gameObject.GetComponent<YoutubePlayer>();
                        currentFocused = listYT[i].gameObject.GetComponent<YoutubePlayer>();
                        distanceToFocusedVid = currentDistance;
                        //Debug.Log("Found first video");
                    }
                    //Debug.Log("first is : " + listYT[i].name);
                    isFocusedOnVid = true;
                    currentFocused.enabled = true;
                    return;
                }
            } else {
                if(currentDistance < distanceThreshold && currentFocused != listYT[i])
                {
                    //Debug.LogError("Happened " + ++count);
                    SetFocus(listYT[i]);   
                    distanceToFocusedVid = currentDistance;                 
                }
            }
        }
    }

    private void SetFocus(YoutubePlayer focusYT)
    {      
        //prevFocused.enabled = false;
        //prevFocused.gameObject.GetComponent<UnityEngine.Video.VideoPlayer>().enabled = false;
        prevFocused.Stop();
        Debug.Log("Stopping Video: " + prevFocused.name);
        if(currentFocused != focusYT)
            prevFocused = currentFocused;
        currentFocused.Stop();
        
        currentFocused = focusYT;
        currentFocused.enabled = true;
        currentFocused.gameObject.GetComponent<UnityEngine.Video.VideoPlayer>().enabled = true;
        //if(!currentFocused.videoPlayer.isPlaying && currentFocused.videoPlayer.isPrepared)
        currentFocused.Start();
//            currentFocused.PlayYoutubeVideo();

    }

    #endif

}
