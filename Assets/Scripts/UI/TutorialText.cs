using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialText : MonoBehaviour
{
    public GameObject[] previousTutorialText;

    // Start is called before the first frame update
    void Start()
    {
        foreach(GameObject g in previousTutorialText)
        {
            g.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
