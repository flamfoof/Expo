using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Byn.Awrtc.Unity;

public class UIControlsDemo : MonoBehaviour
{
    public GameObject mainLogin;
    public GameObject signUp;
    public GameObject characterSelect;
    public GameObject worldSelect;

    public Text feedbackText;
    public GameObject loadingIcon;
    public Button submitLoginButton;
    public ToggleGroup genderToggle;
    public Toggle[] genderList;
    public GameObject[] selectAvatarObj;
    public int selectedAvatarIndex = 0;
    public int selectedHeadIndex = 0;

    public Text characterFeedbackText;


    private void Start()
    {
        if (!loadingIcon)
        {
            Debug.LogWarning("We need an loading animation here, or is missing." +
            "\n Certain animation features won't be available");
        }
    }

    //will be expanded upon
}
