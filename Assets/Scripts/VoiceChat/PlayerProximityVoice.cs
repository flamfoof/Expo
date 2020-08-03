using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Byn.Awrtc;

public class PlayerProximityVoice : MonoBehaviourPunCallbacks
{
    IgniteGameManager gameManager;
    AudioCall audioCall;
    public float minDistance = 3.0f;
    public float maxDistance = 12.0f;
    PhotonView playerPV;

    public float volume = 1.0f;

    void Start()
    {
        gameManager = IgniteGameManager.IgniteInstance;
        audioCall = IgniteGameManager.voiceManager.webRTC;
        if(photonView.IsMine)
        {
            playerPV = photonView;
            InvokeRepeating("ProximitizeVoice", 0.1f, 0.1f);
        }
    }

    void ProximitizeVoice()
    {
        List<PhotonView> listPV = gameManager.GetPlayerList();
        for(int i = 0; i < listPV.Count; i++)
        {
            if(!listPV[i].IsMine)
            {
                float currentDistFromPlayer = CheckPlayerDistance(playerPV, listPV[i]);
                float volumeTarget = 0.0f;
                if(currentDistFromPlayer < minDistance)
                {
                    volumeTarget = 0.0f;                    
                } else if(currentDistFromPlayer > maxDistance)
                {
                    volumeTarget = 1.0f;
                } else 
                {
                    float topDistance = maxDistance - minDistance;
                    volumeTarget = currentDistFromPlayer / topDistance;
                    volumeTarget = Mathf.Abs(volumeTarget - 1.0f);
                }
                //Debug.Log("Volume of " + listPV[i].name + " is now: " + volumeTarget);
                SetVolumeLevels(volumeTarget, listPV[i].GetComponent<PlayerVoiceID>().id);                
            }                
        }
    }

    float CheckPlayerDistance(PhotonView thisPV, PhotonView otherPV)
    {
        float distance = Vector3.Distance(thisPV.transform.position, otherPV.transform.position);
        return distance;
    }

    void SetVolumeLevels(float volume, int voiceID)
    {
        audioCall.SetVolume(volume, voiceID);
    }

}
