using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectVoicePlatform : MonoBehaviour
{
    public GameObject webglVoice;

    void Start()
    {
        #if UNITY_WEBGL
        webglVoice.SetActive(true);
        #endif
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
