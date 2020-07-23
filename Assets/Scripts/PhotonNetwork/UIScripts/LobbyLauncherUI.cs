using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.EventSystems;

public class LobbyLauncherUI : MonoBehaviour
{
    public UIControlsDemo UIControls;
    private LobbyLauncher lobbyLauncher;

    public AssignPlayerAvatar assignPlayerAva;
    public bool hasSelectedAvatar = false;

    #region Public Variable
    [SerializeField]private InputField usernameInput;
    [SerializeField]private InputField emailInput;
    [SerializeField]private InputField passwordInput;
    [SerializeField]private Text nameFeedbackTxt;
    [SerializeField] private Text emailFeedbackTxt;
    [SerializeField] private Text passwordFeedbackTxt;
    #endregion

    public static string playerUsername = "Username";
    public static string playerNickname = "Name";
    
    //for testing purposes
    public bool skipName = false;
    public bool skipEmail = false;
    public bool skipPass = false;
    EventSystem system;


    private void Awake() {
        lobbyLauncher = GetComponent<LobbyLauncher>();
        UIControls = GameObject.FindObjectOfType<UIControlsDemo>();        
    }
    void Start()
    {
        system = EventSystem.current;
        Debug.Log("The name........." +PlayerPrefs.GetString(playerNickname));
        Debug.Log("The email.............."+PlayerPrefs.GetString("Email"));
        Debug.Log("The Password............."+PlayerPrefs.GetString("Password"));
        string defaultName = "";
        assignPlayerAva = GameObject.FindObjectOfType<AssignPlayerAvatar>();
    
        if(!UIControls)
        {
            Debug.LogError("Unable to find UI controls script");
        }

        //sets default previous username if available
        if(usernameInput)
        {
            if(PlayerPrefs.HasKey(playerNickname) && PlayerPrefs.GetString(playerNickname) != "")
            {
                defaultName = PlayerPrefs.GetString(playerNickname);
                usernameInput.text = defaultName;
                lobbyLauncher.validUsername = true;
                lobbyLauncher.nickname = defaultName;
                PhotonNetwork.NickName = defaultName;
                assignPlayerAva.SetPlayerID(defaultName);  
                Debug.Log("PHOTON NICK NAME: " + PhotonNetwork.NickName);
                Debug.Log("PREFS NICK NAME: " + defaultName);
            }
        }


        //add listener to the username input field
        //usernameInput.onValueChanged.AddListener(delegate { CheckName(); });

        //emailInput.onValueChanged.AddListener(delegate { CheckEmail(); });
        //passwordInput.onValueChanged.AddListener(delegate { CheckPassword(); });

        //Add listener to button to start connecting to server
        UIControls.submitLoginButton.onClick.AddListener(delegate {lobbyLauncher.Connect();});

        foreach(Toggle characterToggle in UIControls.genderList)
        {
            characterToggle.onValueChanged.AddListener(delegate {CheckGender(UIControls.genderList);});
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            Selectable next = system.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnDown();

            if (next != null)
            {
                InputField inputfield = next.GetComponent<InputField>();
                if (inputfield != null)
                    inputfield.OnPointerClick(new PointerEventData(system));

                system.SetSelectedGameObject(next.gameObject, new BaseEventData(system));
            }

            //Here is the navigating back part:
            else
            {
                next = Selectable.allSelectablesArray[0];
                system.SetSelectedGameObject(next.gameObject, new BaseEventData(system));
            }

        }
    }
    public void SetPlayerNickname(string name)
    {
        if(string.IsNullOrEmpty(name))
        {
            Debug.LogError("Name is empty/null");
            lobbyLauncher.validUsername = false;
            UIControls.feedbackText.text = "Name cannot be empty";
            return;
        }
        else if(name.Length < 2 || name.Length > 12)
        {
            lobbyLauncher.validUsername = false;
            UIControls.feedbackText.text = "Name must be greater than 2 characters, and be 12 or less characters.";
            return;
        }
        else {
            UIControls.feedbackText.text = "Name is valid";        
            lobbyLauncher.validUsername = true;
            lobbyLauncher.nickname = name;   
            assignPlayerAva.SetPlayerID(name);         
        }
        
    }
    public GenderList.genders CheckGender(Toggle[] genderList)
    {
        for(int i = 0; i < genderList.Length; i++)
        {
            if(genderList[i].isOn)
            {
                hasSelectedAvatar = true;
                switch(genderList[i].GetComponent<GenderList>().gender)
                {
                    case GenderList.genders.Male1: 
                        Debug.Log("Male1");
                        assignPlayerAva.Gender = GenderList.genders.Male1;
                        return GenderList.genders.Male1;
                    case GenderList.genders.Female1: 
                        Debug.Log("Female1");
                        assignPlayerAva.Gender = GenderList.genders.Female1;
                        return GenderList.genders.Female1;
                    case GenderList.genders.Male2:
                        Debug.Log("Male2");
                        assignPlayerAva.Gender = GenderList.genders.Male2;
                        return GenderList.genders.Male2;
                    case GenderList.genders.Female2:
                        Debug.Log("Female2");
                        assignPlayerAva.Gender = GenderList.genders.Female2;
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

    public void SubmitButtonClick()
    {
        if(/*CheckName() && */CheckEmail() && CheckPassword())
        {
            UIControls.mainLogin.SetActive(false);
            UIControls.worldSelect.SetActive(true);
        }
    }
    public void CreateButtonClick()
    {
        UIControls.mainLogin.SetActive(false);
        UIControls.signUp.SetActive(true);
    }
    private bool CheckName()
    {
        if(skipName)
        {
            nameFeedbackTxt.text = "Name is valid";
            SetPlayerNickname(usernameInput.text);
            return true;
        }
        
        //Name Checking
        if(string.IsNullOrEmpty(usernameInput.text))
        {
            nameFeedbackTxt.text = "Please Enter User Name";
            return false;
        }
        else if(PlayerPrefs.GetString("Name","") != usernameInput.text)
        {
            nameFeedbackTxt.text = "Please Enter Correct Name";
            return false;
        }
        else
        {
            nameFeedbackTxt.text = "Name is Valid";
            SetPlayerNickname(usernameInput.text);
            return true;    
        }
    }
    private bool CheckEmail()
    {
        if(skipEmail)
        {
            if(emailInput.text == "ignite")
            {
                emailFeedbackTxt.text = "Ignite!";
                return true;
            }
        }
        
        //Email Checking
        if (string.IsNullOrEmpty(emailInput.text))
        {
            emailFeedbackTxt.text = "Please Enter Email";
            return false;
        }
        else if (PlayerPrefs.GetString("Email", "") != emailInput.text)
        {
            emailFeedbackTxt.text = "Please Enter Correct Email";
            return false;
        }
        else
        {
            emailFeedbackTxt.text = "Email is Valid";
            return true;
        }
    }
    private bool CheckPassword()
    {
        if(skipPass)
        {
            if(passwordInput.text == "ignite")
            {
                passwordFeedbackTxt.text = "Ignite!";
                return true;
            }
            return true;
        }

        if (string.IsNullOrEmpty(passwordInput.text))
        {
            passwordFeedbackTxt.text = "Please Enter Password";
            return false;
        }
        else if (PlayerPrefs.GetString("Password", "") != passwordInput.text)
        {
            passwordFeedbackTxt.text = "Please Enter Correct Password";
            return false;
        }
        else
        {
            passwordFeedbackTxt.text = "Password is Valid";
            return true;
        }
    }
}
