using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using WeIgnite;

//inherited from Ignite Analytics monobehaviour pun callback
public class GameplayAnalytics : IgniteAnalytics
{
    TimerScript timer;
    List<Interactables> interactableList;

    void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        DontDestroyOnLoad(this.gameObject);
    }

    //In the case that there are new interactables being added or when scene changes.
    void UpdateInteractableList()
    {
        Interactables[] tempList = FindObjectsOfType<Interactables>();
        if(interactableList != null)
        {
            interactableList.Clear();
        } else {
            return;
        }
            

        for(int i = 0; i < tempList.Length; i++)
        {
            //In this case, not all interactables will be tracked
            //Will also be dependent on the type of event
            //TODO: filter the interactables to be added
            //      This will be done in the future when we have   
            //      completely laid out our interactables
            interactableList.Add(tempList[i]);
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //update anything else when scene loads, like the timer

        //Times when activated is kept in the class. It will contain
        //its type and other unique identifications. Not all interactables
        //will be submitted to the analytics
        UpdateInteractableList();

        timer.StartTimer();
    }

    //TODO: Send analytics to Unity Analytics
    //      Gather all the analytics data and send them all at intervals
    //      of 5 seconds? We can timestamp when each variable changes  
    //      then send them in a single push.
    void SendTimePayload(/*parameters here, like time, interactable list data, etc.*/)
    {

    }
    
    //In cases like OnApplicationQuit, send all the data before logging off
    void SendAllPayload()
    {

    }
}
