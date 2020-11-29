using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Voice.PUN;
using UnityEngine.UI;
using UnityEngine.Networking;
using Photon.Realtime;

public class PlayerNameShow : MonoBehaviourPunCallbacks
{
    GameObject localPlayer;
    PhotonVoiceView photonVoiceView;
    IgniteGameManager gm;
    public GameObject playerName;
    public GameObject playerOrganization;
    public GameObject infoUI;
    public bool infoUIEnabled;
    public Image speakerImage;
    public Image recorderImage;
    public RawImage profilePic;
    public bool startedRetrievingPic = false;
    int actorNumber; 

    // Start is called before the first frame update
    void Start()
    {
        localPlayer = IgniteGameManager.localPlayer;
        gm = IgniteGameManager.IgniteInstance;
        photonVoiceView = GetComponentInParent<PhotonVoiceView>();

        foreach (PhotonView pv in gm.GetPlayerList())
        {
            //Show the name and organization from PlayerPrefs
            //pv.GetComponent<UserActions>().playerName.text = pv.Owner.NickName;
            //pv.GetComponent<UserActions>().playerOrganization.text = PlayerPrefs.GetString("Organization", "");
        }
    }

    void Update()
    {
        transform.LookAt(localPlayer.transform, Vector3.up);
        //transform.rotation.SetLookRotation(localPlayer.transform.position, Vector3.up);

        /*
        foreach(PhotonView pv in gm.GetPlayerList())
        {
            if(!pv.IsMine)
            {
                try{    
                    pv.GetComponent<UserActions>().playerName.transform.LookAt(IgniteGameManager.localPlayer.transform, Vector3.up);
                    pv.GetComponent<UserActions>().playerOrganization.transform.LookAt(IgniteGameManager.localPlayer.transform, Vector3.up);
                }
                catch (Exception e) {
                    Debug.Log("Exception when finding aiming name: " + e);
                    Debug.Log("player in player list not found");
                    gm.RefreshPlayerList();
                }
            }
        }*/

        speakerImage.enabled = photonVoiceView.IsSpeaking;
        recorderImage.enabled = photonVoiceView.IsRecording;
    }

    public void EnablePlayerButtonInfoUI()
    {
        infoUI.SetActive(true);
        infoUIEnabled = true;
        playerName.SetActive(false);
        playerOrganization.SetActive(false);
        DisplayProfileImage();
    }

    //public void DisablePlayerButtonInfoUI()
    //{
    //    infoUI.SetActive(false);
    //    infoUIEnabled = false;
    //    playerName.SetActive(true);
    //    playerOrganization.SetActive(true);
    //}

    //stephen code
    public IEnumerator IEDisablePlayerButtonInfoUI()
    {
        yield return new WaitForSeconds(8.0f);

        infoUI.SetActive(false);
        infoUIEnabled = false;
        playerName.SetActive(true);
        //playerOrganization.SetActive(true);
    }

    public void DisablePlayerButtonInfoUI()
    {        
        StartCoroutine(IEDisablePlayerButtonInfoUI());
    }
    //stephen code end

    public void DisplayProfileImage()
    {
        if(!startedRetrievingPic)
        {
            startedRetrievingPic = true;
            Debug.Log("Starting to diplay profile");
            StartCoroutine(RetrieveProfilePic());
        }        
    }

    IEnumerator RetrieveProfilePic()
    {
        Debug.Log("Started www for profile");
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(IgniteGameManager.IgniteInstance.serverProfileURL + 
                                                                playerName.GetComponent<TextMesh>().text + 
                                                                ".jpg");
        Debug.Log(IgniteGameManager.IgniteInstance.serverProfileURL + 
                                                                playerName.GetComponent<TextMesh>().text + 
                                                                ".jpg");

        yield return www.SendWebRequest();

        if(www.isNetworkError || www.isHttpError) {
            Debug.Log(www.error);
            startedRetrievingPic = false;
        }
        else {
            Texture myTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;
            profilePic.texture = myTexture;
        }

    }
}
