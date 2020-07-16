using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Byn.Unity.Examples;

public class AskVoicePermission : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(ExampleGlobals.RequestPermissions(true, false));
    }
}
