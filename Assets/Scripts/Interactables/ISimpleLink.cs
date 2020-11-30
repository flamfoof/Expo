using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.InputSystem;

public class ISimpleLink : Interactables
{
    public string linkURL;
    public bool isSlide = false;
    public bool canClick = true;
    [DllImport("__Internal")]
	private static extern void openWindow(string url);
    
    public override void Perform(InputActionPhase phase)
    {
        //print("**PHASE " +phase);
        if(phase == InputActionPhase.Started)
        {
            TimesUsed++;
            if(canClick)
            {
                
                canClick = !canClick;

                if (BBBAnalytics.instance)
                {
                    BBBAnalytics.instance.ClickedWeb(linkURL);
                }
                Debug.Log("Opening link");
                //Application.OpenURL(linkURL);
                #if UNITY_EDITOR
                Application.OpenURL(linkURL);
                #elif UNITY_WEBGL //maybe else if for webgl
                openWindow(linkURL);
                #else 
                Application.OpenURL(linkURL);
                #endif

            } else 
            {
                canClick = !canClick;
            }
            
            
        }        
    }
}
