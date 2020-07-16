using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Byn.Unity.Examples;

public class AskVoicePermission : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(ExampleGlobals.RequestPermissions(true, false));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
