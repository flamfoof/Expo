using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class LobbyLauncherUI : MonoBehaviour
{
    public UIControlsDemo UIControls;
    private LobbyLauncher lobbyLauncher;

    public static string playerUsername = "Username";
    public static string playerNickname = "Nickname";


    private void Awake() {
        lobbyLauncher = GetComponent<LobbyLauncher>();
        UIControls = GameObject.FindObjectOfType<UIControlsDemo>();        
    }
    void Start()
    {
        string defaultName = "";
    
        if(!UIControls)
        {
            Debug.LogError("Unable to find UI controls script");
        }

        //sets default previous username if available
        if(UIControls.usernameInput)
        {
            if(PlayerPrefs.HasKey(playerNickname) && PlayerPrefs.GetString(playerNickname) != "")
            {
                defaultName = PlayerPrefs.GetString(playerNickname);
                UIControls.usernameInput.text = defaultName;
                lobbyLauncher.validUsername = true;
                Debug.Log("PHOTON NICK NAME: " + PhotonNetwork.NickName);
                Debug.Log("PREFS NICK NAME: " + defaultName);
            }
        }

        PhotonNetwork.NickName = defaultName;

        //add listener to the username input field
        UIControls.usernameInput.onValueChange.AddListener(delegate {SetPlayerNickname(UIControls.usernameInput.text);});
        
        //TODO: add listner to the password input field maybe

        //Add listener to button to start connecting to server
        UIControls.submitLoginButton.onClick.AddListener(delegate {lobbyLauncher.Connect();});

    }

    public void SetPlayerNickname(string name)
    {
        if(string.IsNullOrEmpty(name))
        {
            Debug.LogError("Name is empty/null");
            lobbyLauncher.validUsername = false;
            UIControls.feedbackText.text = "Name cannot be empty";
            return;
        } else if(name.Length < 2 || name.Length > 12)
        {
            lobbyLauncher.validUsername = false;
            UIControls.feedbackText.text = "Name must be greater than 2 characters, and be 12 or less characters.";
            return;
        } else {
            UIControls.feedbackText.text = "Name is valid";        
            lobbyLauncher.validUsername = true;
            lobbyLauncher.nickname = name;
        }
        
    }

    public void SubmitLogin()
    {
        //TODO: Get username and password and compare them to the database
        //      logins

    }

    public GenderList.genders CheckGender(Toggle[] genderList)
    {
        for(int i = 0; i < genderList.Length; i++)
        {
            if(genderList[i].isOn)
            {
                switch(genderList[i].GetComponent<GenderList>().gender)
                {
                    case GenderList.genders.Male1: 
                        Debug.Log("Male");
                        return GenderList.genders.Male1;
                    case GenderList.genders.Female1: 
                        Debug.Log("Female");
                        return GenderList.genders.Female1;
                    case GenderList.genders.Male2:
                        Debug.Log("Male");
                        return GenderList.genders.Male2;
                    case GenderList.genders.Female2:
                        Debug.Log("Female");
                        return GenderList.genders.Female2;
                    case GenderList.genders.NonBinary: 
                        Debug.Log("Non-binary");
                        return GenderList.genders.NonBinary;
                    default: 
                        break;
                }
            }            
        }
        return GenderList.genders.None;
    }

    public void PlayLoadAnimation(bool isActive)
    {        
        if(UIControls.loadingIcon)
        {
            UIControls.loadingIcon.SetActive(isActive);
        } else {
            Debug.LogWarning("Unable to play load animation. Loading Icon is missing");
        }
        
    }
}
