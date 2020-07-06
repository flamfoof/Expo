using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.InputSystem;

public class ISimpleLink : Interactables
{
    public string linkURL;

    [DllImport("__Internal")]
	private static extern void openWindow(string url);

    public override void Perform(InputActionPhase phase)
    {

        //Application.OpenURL(linkURL);
        #if UNITY_EDITOR
        Application.OpenURL(linkURL);
        #endif
        openWindow(linkURL);
    }
}
