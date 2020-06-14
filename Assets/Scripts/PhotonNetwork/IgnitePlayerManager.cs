using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class IgnitePlayerManager : MonoBehaviourPunCallbacks
{
    public static GameObject LocalPlayerInstance;

    public GameObject playerUI;

    private void Awake() 
    {
        if(photonView.IsMine)
        {
            LocalPlayerInstance = gameObject;
        }

        DontDestroyOnLoad(gameObject);
    }

    private void Start() 
    {
        //CameraWork
    }
}
