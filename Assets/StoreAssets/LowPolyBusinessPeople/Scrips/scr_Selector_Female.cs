using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;


public class scr_Selector_Female : MonoBehaviour {

    //public Renderer[] suits;
    
    private int pick = 0;
    private int count = 0;
    private int type = 0;
    public List<GameObject> Suit;
    public List<GameObject> SkinSuit;
    public List<GameObject> Skirts;
    public List<GameObject> SkinSkirts;
    private Renderer oRenderer;
    public int headSelected;
    public int bodySelected;
    public int skirtOrPantsSelected;


    // Use this for initialization
    void Start()
    {
        // populate list based on tags
        /*
        foreach (Transform child in transform)
        {
            if (child.tag == "Female_SkinForTrousers")
                {
                    SkinSuit.Add(child.gameObject);
                    //Debug.Log(child + " added");
                }

            if (child.tag == "Female_SkinForSkirt")
                {
                    SkinSkirts.Add(child.gameObject);
                    //Debug.Log(child + " added");
                }
            if (child.tag == "Female_TrouserSuit")
            {
                Suit.Add(child.gameObject);
                //Debug.Log(child + " added");
            }

            if (child.tag == "Female_SkirtSuit")
            {
                Skirts.Add(child.gameObject);
                //Debug.Log(child + " added");
            }


        }*/

        if(SessionHandler.instance.CheckIfPresenter() || SessionHandler.instance.CheckIfStaff())
        {
            headSelected = 1;
        }
        // Decide which type
        pickType(skirtOrPantsSelected);
        //pick a suit
        pickSuit(headSelected);
        // pick headType A/B
        pickSkin(bodySelected);
   
    }

    void pickType(int typeIndex)
    {
        type = typeIndex; // suit or skirt
        //Debug.Log(type);
    }


    // Function for picking suits
    public void pickSuit(int suitIndex)
    {
        if (type == 0) // 0 is trouser version
        {
            // clear others
            /*
            foreach (GameObject o in Skirts)
            {
                oRenderer = o.GetComponentInChildren<Renderer>();
                oRenderer.enabled = false;
            }*/
            
            for(int i = 0; i < Skirts.Count; i++)
            {
                oRenderer = Skirts[i].GetComponentInChildren<Renderer>();
                oRenderer.enabled = false;
            }


            pick = suitIndex;//Random.Range(0, Suit.Count);
            count = 0;
            for(int i = 0; i < Suit.Count; i++)
            {
                if (i == pick)
                {
                    oRenderer = Suit[i].GetComponentInChildren<Renderer>();
                    oRenderer.enabled = true;
                }
                else
                {
                    oRenderer = Suit[i].GetComponentInChildren<Renderer>();
                    oRenderer.enabled = false;
                }
            }
            /* //This makes it random in selection
            foreach (GameObject o in Suit)
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

        if (type == 1) // 1 is skirt version
        {            
            /*
            foreach (GameObject o in Skirts)
            {
                oRenderer = o.GetComponentInChildren<Renderer>();
                oRenderer.enabled = false;
            }*/
            
            for(int i = 0; i < Skirts.Count; i++)
            {
                oRenderer = Skirts[i].GetComponentInChildren<Renderer>();
                oRenderer.enabled = false;
            }


            pick = Random.Range(0, Skirts.Count);
            count = 0;
            for(int i = 0; i < Suit.Count; i++)
            {
                if (i == pick)
                {
                    oRenderer = Suit[i].GetComponentInChildren<Renderer>();
                    oRenderer.enabled = true;
                }
                else
                {
                    oRenderer = Suit[i].GetComponentInChildren<Renderer>();
                    oRenderer.enabled = false;
                }
            }
            /* //This makes it random in selection
            foreach (GameObject o in Suit)
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


    }




    // Function for picking skins
    public void pickSkin(int skinIndex)
    {

        if (type == 0) // 0 is trouser version
        {
            for(int i = 0; i < SkinSkirts.Count; i++)
            {
                oRenderer = SkinSkirts[i].GetComponentInChildren<Renderer>();
                oRenderer.enabled = false;
            }

            /*
            foreach (GameObject o in SkinSkirts)
            {
                oRenderer = o.GetComponentInChildren<Renderer>();
                oRenderer.enabled = false;
            }*/

            pick = skinIndex;// Random.Range(0, SkinSuit.Count);
            
            count = 0;
            for(int i = 0; i < SkinSuit.Count; i++)
            {
                if (i == pick)
                {
                    oRenderer = SkinSuit[i].GetComponentInChildren<Renderer>();
                    oRenderer.enabled = true;
                }
                else
                {
                    oRenderer = SkinSuit[i].GetComponentInChildren<Renderer>();
                    oRenderer.enabled = false;
                }
                count++;
            }
            /*
            foreach (GameObject o in SkinSuit)
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

        if (type == 1) // 1 is skirt version
        {
            for(int i = 0; i < SkinSkirts.Count; i++)
            {
                oRenderer = SkinSkirts[i].GetComponentInChildren<Renderer>();
                oRenderer.enabled = false;
            }

            /*
            foreach (GameObject o in SkinSkirts)
            {
                oRenderer = o.GetComponentInChildren<Renderer>();
                oRenderer.enabled = false;
            }*/

            pick = Random.Range(0, SkinSkirts.Count);
            
            count = 0;
            for(int i = 0; i < SkinSuit.Count; i++)
            {
                if (i == pick)
                {
                    oRenderer = SkinSuit[i].GetComponentInChildren<Renderer>();
                    oRenderer.enabled = true;
                }
                else
                {
                    oRenderer = SkinSuit[i].GetComponentInChildren<Renderer>();
                    oRenderer.enabled = false;
                }
                count++;
            }
            /*
            foreach (GameObject o in SkinSuit)
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
    }



    // Update is called once per frame
    void Update () {

       /* if (Input.GetKeyDown("space"))
        {


            pickType();
            // pick a suit
            pickSuit();
            // pick headType A/B
            pickSkin();
       

        }*/
    }


}

