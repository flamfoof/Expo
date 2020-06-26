using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YTVidDistancePlay : MonoBehaviour
{
    public GameObject player;
    public float distanceThreshold = 6.0f;
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
            listYT.Add(vid);
        }
        Invoke("FindPlayer", 1);
//        Debug.Log("started youtube videos");
        #if UNITY_WEBGL
        //Calls an update every 0.5 seconds to this function
        InvokeRepeating("CheckPlayerDistance", 2.0f, 0.8f);

        #endif
    }

    void FindPlayer()
    {
        player = GameObject.FindObjectOfType<UserActions>().gameObject;
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
            //Debug.Log("Distance: " + listYT[i].name + Vector3.Distance(listYT[i].transform.position, player.transform.position));
            if(!currentFocused)
            {
                if(Vector3.Distance(listYT[i].transform.position, player.transform.position) < distanceThreshold && !firstFocused)
                {
                    for(int j = 0; j < listYT.Count; j++)
                    {
                        firstFocused = true;
                        prevFocused = listYT[i].gameObject.GetComponent<YoutubePlayer>();
                        currentFocused = listYT[i].gameObject.GetComponent<YoutubePlayer>();
                        //Debug.Log("Found first video");
                    }
                    //Debug.Log("first is : " + listYT[i].name);
                    isFocusedOnVid = true;
                    currentFocused.enabled = true;
                    return;
                }
            } else {
                if(Vector3.Distance(listYT[i].gameObject.transform.position, player.transform.position) < distanceThreshold && currentFocused != listYT[i])
                {
                    //Debug.LogError("Happened " + ++count);
                    SetFocus(listYT[i]);                    
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
