using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIControlsDemo : MonoBehaviour
{
    public Text feedbackText;
    public GameObject loadingIcon;
    public Button submitLoginButton;
    public InputField usernameInput;
    public InputField passwordInput;
    public ToggleGroup genderToggle;
    public Toggle[] genderList;
    

    private void Start() {    
        if(!loadingIcon)
        {
            Debug.LogWarning("We need an loading animation here, or is missing." + 
            "\n Certain animation features won't be available");
        }
    }

    public GenderList.genders CheckGender()
    {
        for(int i = 0; i < genderList.Length; i++)
        {
            if(genderList[i].isOn)
            {
                switch(genderList[i].GetComponent<GenderList>().gender)
                {
                    case GenderList.genders.Male: 
                        Debug.Log("Male");
                        return GenderList.genders.Male;
                    case GenderList.genders.Female: 
                        Debug.Log("Female");
                        return GenderList.genders.Female;
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

    //will be expanded upon
}
