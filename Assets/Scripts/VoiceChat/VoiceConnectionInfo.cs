using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Byn.Unity.Examples;

public class VoiceConnectionInfo : MonoBehaviour
{
    public Image iconOn;
    public bool startedAnim = false;
    public bool isChecking = true;
    public float animTime = 1.0f;
    public UIEffectsUtils UIEffects;
    IEnumerator fadeRepeat;
    public bool isMicConnected = false;
    public bool IsMicConnected 
    {
        get
        {
            return this.isMicConnected;
        }

        set
        {                    
            if(isMicConnected != value && isMicConnected == false)
            {
                startedAnim = true;
                Debug.Log("Microphone is enabled. Turning on smiley face. :D");
                StartCoroutine(fadeRepeat);
            } else if(isMicConnected != value && isMicConnected == true)
            {
                startedAnim = false;
                Debug.Log("Microphone has disconnected. Hiding the voice enabled icon");
                StopCoroutine(fadeRepeat);
                StartCoroutine(UIEffects.StopFadeRepeat(iconOn, animTime));
            }

            isMicConnected = value;
            
        }
    }

    void Start() 
    {
        UIEffects = GameObject.FindObjectOfType<UIEffectsUtils>();
        fadeRepeat = UIEffects.FadeRepeat(iconOn, animTime);

        StartCoroutine(CheckVoiceConnection());
    }

    IEnumerator CheckVoiceConnection()
    {
        yield return new WaitForFixedUpdate();

        while(isChecking)
        {
            yield return new WaitForSeconds(0.5f);
            IsMicConnected = ExampleGlobals.HasAudioPermission();
        }
        
            
    }


}
