using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using System;

public class scr_Selector : MonoBehaviour {

    //public Renderer[] suits;
    
    private int pick = 0;
    private int count = 0;
    public List<GameObject> Suits;
    public List<GameObject> Heads;
    private Renderer oRenderer;
    public int headSelected;
    public int bodySelected;

    // Use this for initialization
    void Start()
    {

        // populate list based on tags
        foreach (Transform child in transform)
        {
            if (child.tag == "MaleSuit")
                {
                    Suits.Add(child.gameObject);
                    //Debug.Log(child + " added");
                }

            if (child.tag == "MaleHead")
                {
                    Heads.Add(child.gameObject);
                    //Debug.Log(child + " added");
                }

           
        }

        //pick a suit
        pickOneSuit(bodySelected);
        // pick headType A/B
        pickOneHead(headSelected);
   
    }

    

    // Function for picking suits at random
    void pickSuit()
        {
            pick = UnityEngine.Random.Range(0, Suits.Count);
            count = 0;

            foreach (GameObject o in Suits)
            {

                if (count == pick)
                {
                    oRenderer = o.GetComponentInChildren<Renderer>();
                    oRenderer.enabled = true;
                }
                else
                {
                    oRenderer = o.GetComponentInChildren<Renderer>();
                    oRenderer.enabled = false;
                }
                count++;
            }
        }

    public void ChangeHead(int headIndex)
    {
        pickOneHead(headSelected);
    }
    public void ChangeBody(int headIndex)
    {
        pickOneSuit(bodySelected);
    }

    // Function for picking suits 
    void pickOneSuit(int suitIndex)
    {
        pick = suitIndex;
        count = 0;

        foreach (GameObject o in Suits)
        {

            if (count == pick)
            {
                oRenderer = o.GetComponentInChildren<Renderer>();
                oRenderer.enabled = true;
            }
            else
            {
                oRenderer = o.GetComponentInChildren<Renderer>();
                oRenderer.enabled = false;
            }
            count++;
        }
    }



    // Function for picking heads and hands will be picked to match based on choice here too.
    void pickHead()
    {



                // now pick a head // the choice here is important to remeber so that we can choose hair styles that suit.
                pick = UnityEngine.Random.Range(0, Heads.Count);
                
                count = 0;

                foreach (GameObject o in Heads)
                {

                    if (count == pick)
                    {
                        oRenderer = o.GetComponentInChildren<Renderer>();
                        oRenderer.enabled = true;
                    }
                    else
                    {
                        oRenderer = o.GetComponentInChildren<Renderer>();
                        oRenderer.enabled = false;
                    }
                    count++;
                }
       
        

    }

    // Function for picking heads and hands will be picked to match based on choice here too.
    void pickOneHead(int headIndex)
    {
        // now pick a head // the choice here is important to remeber so that we can choose hair styles that suit.
        pick = headIndex;

        count = 0;

        foreach (GameObject o in Heads)
        {

            if (count == pick)
            {
                oRenderer = o.GetComponentInChildren<Renderer>();
                oRenderer.enabled = true;
            }
            else
            {
                oRenderer = o.GetComponentInChildren<Renderer>();
                oRenderer.enabled = false;
            }
            count++;
        }



    }

    // Update is called once per frame
    void Update () {

        if (Input.GetKeyDown("space"))
        {

                  
          
            // pick a suit
            pickSuit();
            // pick headType A/B
            pickHead();
       

        }
    }


}

