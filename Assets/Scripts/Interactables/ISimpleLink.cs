using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.InputSystem;

public class ISimpleLink : Interactables
{
    public string linkURL;
    public IgniteAnalytics analytics;
    public bool isSlide = false;
    [DllImport("__Internal")]
	private static extern void openWindow(string url);
    
    public override void Perform(InputActionPhase phase)
    {

        if(phase == InputActionPhase.Performed)
        {
            TimesUsed++;
            if(analytics)
            {
                analytics.ClickedStats(linkURL);
            }

            //Application.OpenURL(linkURL);
            #if UNITY_EDITOR
            Application.OpenURL(linkURL);
            #else //maybe else if for webgl
            openWindow(linkURL);
            #endif
            
        }        
    }
}
