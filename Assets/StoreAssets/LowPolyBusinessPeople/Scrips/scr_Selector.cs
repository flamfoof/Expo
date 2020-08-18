using System.Collections.Generic;
using UnityEngine;

public class scr_Selector : MonoBehaviour
{

    //public Renderer[] suits;

    private int pick = 0;
    private int count = 0;
    public List<GameObject> Suits;
    public List<GameObject> Heads;
    private Renderer oRenderer;

    AssignPlayerAvatar AvaInfo;
    // Use this for initialization
    void Start()
    {
        AvaInfo = GameObject.FindObjectOfType<AssignPlayerAvatar>();
        // populate list based on tags
        /*foreach (Transform child in transform)
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
        */
        pickOneSuit(AvaInfo.bodyIndex);
        PickOneHead(AvaInfo.headIndex);
    }

    // Function for picking suits at random
    public void pickSuit()
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



    // Function for picking suits 
    public void pickOneSuit(int suitIndex)
    {
        pick = suitIndex;
        count = 0;
        /*
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
        }*/

        for(int i = 0; i < Suits.Count; i++)
        {
            if (i == pick)
            {
                oRenderer = Suits[i].GetComponentInChildren<Renderer>();
                oRenderer.enabled = true;
            }
            else
            {
                oRenderer = Suits[i].GetComponentInChildren<Renderer>();
                oRenderer.enabled = false;
            }
        }
    }

    // Function for picking heads and hands will be picked to match based on choice here too.
    void PickHead()
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
    public void PickOneHead(int headIndex)
    {
        // now pick a head // the choice here is important to remeber so that we can choose hair styles that suit.
        pick = headIndex;
        count = 0;
        for(int i = 0; i < Heads.Count; i++)
        {
            if (i == pick)
            {
                oRenderer = Heads[i].GetComponentInChildren<Renderer>();
                oRenderer.enabled = true;
            }
            else
            {
                oRenderer = Heads[i].GetComponentInChildren<Renderer>();
                oRenderer.enabled = false;
            }
        }

        /*
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
        }*/
    }

    // Update is called once per frame
    void Update()
    {
        /*
        if (Input.GetKeyDown("space"))
        {

                  
          
            // pick a suit
            pickSuit();
            // pick headType A/B
            pickHead();
       

        }*/
    }


}

