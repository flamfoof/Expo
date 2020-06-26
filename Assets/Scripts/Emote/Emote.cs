using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Emote : MonoBehaviourPunCallbacks
{
    public string name;
    public float duration = 5.0f;

    void Start()
    {        
        Destroy(gameObject, duration);
    }

    private void OnDestroy() {
        PhotonNetwork.Destroy(photonView);
    }
}
