using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Byn.Unity.Examples;
using Byn.Awrtc;
using Byn.Awrtc.Unity;
using System;

public class AskVoicePermission : MonoBehaviour
{
    public bool askAtStart = false;
    public GameObject rootUIDialogue;
    public UserPermissionCommunication userAllowPermissions;
    public Toggle micPerm;
    public Toggle vidPerm;
    public int counter = 0;
    protected ICall mCall;
    
    MediaConfig mediaConfig = new MediaConfig();
    NetworkConfig netConfig;

    void Start()
    {
        userAllowPermissions = GameObject.FindObjectOfType<UserPermissionCommunication>();
        
        StartCoroutine(GetProfilePermissions());

        
        netConfig = CreateNetworkConfig();
        mediaConfig.Audio = false;
        mediaConfig.Video = true;
        mediaConfig.Format = FramePixelFormat.ABGR;

        //original lines
        mediaConfig.MinWidth = 160;
        mediaConfig.MinHeight = 120;
        mediaConfig.MaxWidth = 1920;
        mediaConfig.MaxHeight = 1080;

        //will be overwritten by UI in normal use
        mediaConfig.IdealWidth = 160;
        mediaConfig.IdealHeight = 120;
        mediaConfig.IdealFrameRate = 30;

        InitComms();
        
        //if(ExampleGlobals.HasAudioPermission())
    }

    protected virtual void OnCallFactoryReady()
    {
        //set to warning for regular use
        UnityCallFactory.Instance.RequestLogLevel(UnityCallFactory.LogLevel.Info);
    }

    protected virtual void OnCallFactoryFailed(string error)
    {
        string fullErrorMsg = typeof(CallApp).Name + " can't start. The " + typeof(UnityCallFactory).Name + " failed to initialize with following error: " + error;
        Debug.LogError(fullErrorMsg);
    }

    public void ButtonMicPermissionRequest()
    {
        mediaConfig.Audio = true;
        mediaConfig.Video = false;
        
        mCall = CreateCall(netConfig);
        mCall.Configure(mediaConfig);
        userAllowPermissions.SetMicrophone(!userAllowPermissions.allowMicrophone);
        if(userAllowPermissions.allowMicrophone)
        {
            PlayerPrefs.SetInt("AllowMicrophone", 1);
        } else 
        {
            PlayerPrefs.SetInt("AllowMicrophone", 0);
        }
        

        DisposeMCall();
    }

    public void ButtonVidPermissionRequest()
    {
        mediaConfig.Audio = false;
        mediaConfig.Video = true;
        mCall = CreateCall(netConfig);
        mCall.Configure(mediaConfig);
        userAllowPermissions.SetVideo(!userAllowPermissions.allowVideo);
        if(userAllowPermissions.allowVideo)
        {
            PlayerPrefs.SetInt("AllowVideo", 1);
        } else 
        {
            PlayerPrefs.SetInt("AllowVideo", 0);
        }

        DisposeMCall();
    }


    //The ICall and network config are for creating a pseudo permission
    //call. There's no real network connection going on here.
    protected virtual ICall CreateCall(NetworkConfig netConfig){
        //setup the server
        return UnityCallFactory.Instance.Create(netConfig);
    }
    
    public void DisposeMCall()
    {
        mCall.Dispose();
        mCall = null;
        Debug.Log("Triggering garbage collection");
        GC.Collect();
        GC.WaitForPendingFinalizers();
        Debug.Log("Call destroyed");
    }

    public IEnumerator DisposeCall()
    {
        yield return new WaitForSeconds(0.2f);
        mCall.Dispose();
        mCall = null;
        Debug.Log("Triggering garbage collection");
        GC.Collect();
        GC.WaitForPendingFinalizers();
        Debug.Log("Call destroyed");
    }

    public IEnumerator DisposeVidCall()
    {
        Debug.Log("Starting disposal vid");
        yield return new WaitForSeconds(0.2f);
        mCall.Dispose();
        mCall = null;
        Debug.Log("Triggering garbage collection");
        GC.Collect();
        GC.WaitForPendingFinalizers();
        Debug.Log("Call destroyed");
    }

    public void InitComms()
    { 
        #if UNITY_WEBGL
        mCall = CreateCall(netConfig);
        //mCall.Configure(mediaConfig);         
        #endif
    }


    public NetworkConfig CreateNetworkConfig()
    {
        NetworkConfig netConfig = new NetworkConfig();

        if (netConfig.SignalingUrl == "")
        {
            throw new InvalidOperationException("set signaling url is empty");
        }
        return netConfig;
    }


    public IEnumerator GetProfilePermissions()
    {
        yield return new WaitForEndOfFrame();
        /*//keep this if we want players to keep their settings
        if(PlayerPrefs.GetInt("AllowMicrophone") == 1)
        {
            micPerm.isOn = true;
            userAllowPermissions.SetMicrophone(true);            
        } else 
        {
            micPerm.isOn = false;
            userAllowPermissions.SetMicrophone(false);            
        }

        if(PlayerPrefs.GetInt("AllowVideo") == 1)
        {
            vidPerm.isOn = true;
            userAllowPermissions.SetVideo(true);            
        } else 
        {
            vidPerm.isOn = false;
            userAllowPermissions.SetVideo(false);
        }*/
    }

    public void CloseDialogueBox()
    {
        rootUIDialogue.SetActive(false);
    }
}
