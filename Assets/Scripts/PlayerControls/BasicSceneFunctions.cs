using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Byn.Unity.Examples;
using UnityEngine.Audio;

public class BasicSceneFunctions : MonoBehaviour
{
    public AudioMixer mixer;
    
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

    public void ExitApp()
    {
        BBBAnalytics.instance.EndSession(() => {
            Debug.Log("Applicaiton Quit was called");
            Application.Quit();
        });
        
    }

    public void SetLevel(float sliderValue) {
        mixer.SetFloat("MasterVolume", Mathf.Log10(sliderValue) * 20);
    }

}
