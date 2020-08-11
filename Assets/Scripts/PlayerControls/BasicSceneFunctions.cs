using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Byn.Unity.Examples;

public class BasicSceneFunctions : MonoBehaviour
{
    

    public void ChangeScene(string scene)
    {
        IgniteGameManager.IgniteInstance.LeaveRoom();
        SceneManager.LoadScene(scene);
    }

    public void AskVideoPermission()
    {
        StartCoroutine(ExampleGlobals.RequestPermissions(false, true));
    }

    public void AskAudioPermission()
    {
        StartCoroutine(ExampleGlobals.RequestPermissions(true, false));
    }
}
