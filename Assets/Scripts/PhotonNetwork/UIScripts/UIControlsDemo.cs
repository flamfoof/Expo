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

    private void Start() {    
        if(!loadingIcon)
        {
            Debug.LogWarning("We need an loading animation here, or is missing." + 
            "\n Certain animation features won't be available");
        }
        
    }
    //will be expanded upon
}
