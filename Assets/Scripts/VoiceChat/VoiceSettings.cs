using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoiceSettings : MonoBehaviour
{
    public bool lockChatVisibility;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ToggleChatVisibility()
    {
        lockChatVisibility = !lockChatVisibility;
    }

    public void OnClickSend()
	{
        if(GetComponent<SelectVoicePlatform>().webRTCVoice.gameObject.activeSelf)
        {
            GetComponent<SelectVoicePlatform>().webRTCVoice.GetComponent<AudioCall>().SendButtonPressed();
        } else 
        {
            GetComponent<SelectVoicePlatform>().platformVoice.GetComponent<PhotonTextComms>().OnClickSend();
        }
		
	}
}
