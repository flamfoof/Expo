using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleURL : MonoBehaviour
{
    public string url;
    // Start is called before the first frame update
    public void openURL()
    {
        Application.OpenURL(url);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
