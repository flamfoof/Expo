using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandMobileOpen : MonoBehaviour
{
    public GameObject commandButtonList;
    public GameObject commandRingUI;
    public GameObject parent;
    public bool toggleMenu = false;

    private void Start() 
    {
//#if !UNITY_IOS || !UNITY_ANDROID || !UNITY_EDITOR
        //parent.SetActive(false);
//#endif

//#if !UNITY_IOS || !UNITY_ANDROID
        commandRingUI.SetActive(true);
//#endif

    }

    public void OpenCommandsMobile()
    {
        Debug.Log("Opened mobile menu");
        toggleMenu = !toggleMenu;
        commandButtonList.SetActive(toggleMenu);
    }
}
