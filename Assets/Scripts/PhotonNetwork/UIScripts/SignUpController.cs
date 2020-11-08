using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SignUpController : MonoBehaviour
{
    #region Public Variable
    [SerializeField] private InputField nameField;
    [SerializeField] private InputField organizationField;
    [SerializeField] private InputField emailField;
    [SerializeField] private InputField passwordField;
    [SerializeField] private InputField confirmPasswordField;
    [SerializeField] private Text nameFeedbackTxt;
    [SerializeField] private Text emailFeedbackTxt;
    [SerializeField] private Text passwordFeedbackTxt;
    [SerializeField] private Text confirmPasswordFeedbackTxt;
    [SerializeField] private Text organizationFeedbackTxt;
    #endregion
    #region Private Variable
    private UIControlsDemo UIControls;
    private Validate validate;
    #endregion

    #region System Function
    private void Awake()
    {
        UIControls = GameObject.FindObjectOfType<UIControlsDemo>();
        validate = GetComponent<Validate>();
    }
    private void OnEnable()
    {
        RefreshText();
        nameField.onValueChanged.AddListener(delegate { CheckUserName(); });
        organizationField.onValueChanged.AddListener(delegate { CheckOrganization(); });
        emailField.onValueChanged.AddListener(delegate { CheckEmail(); });
        passwordField.onValueChanged.AddListener(delegate { CheckPassword(); });
        confirmPasswordField.onValueChanged.AddListener(delegate { CheckConfirmPassword(); });
    }
    #endregion
    #region Public Methods
    public void CreateButtonClick()
    {
        if(CheckUserName() && CheckOrganization() &&CheckEmail() && CheckPassword() && CheckConfirmPassword())
        {
            PlayerPrefs.SetString("Name",nameField.text);
            PlayerPrefs.SetString("Organization",organizationField.text);
            PlayerPrefs.SetString("Email", emailField.text);
            PlayerPrefs.SetString("Password", passwordField.text);
            RefreshText();
            UIControls.mainLogin.SetActive(true);
            UIControls.signUp.SetActive(false);
            //AnalyticsController.Instance.ProfileInfoAnalytics();
        }
    }
    public void BackButtonClick()
    {
        UIControls.mainLogin.SetActive(true);
        UIControls.signUp.SetActive(false);
    }
    #endregion

    #region Private Methods
    private bool CheckOrganization()
    {
        if(string.IsNullOrEmpty(organizationField.text))
        {
            Debug.Log("Please Enter the Organization");
            organizationFeedbackTxt.text = "Please Enter Organization";
            return false;
        }
        else if (organizationField.text.Length < 12)
        {
            organizationFeedbackTxt.text = "Organization must be greater than 12 characters";
            return false;
        }
        else
        {
            organizationFeedbackTxt.text = "";
            return true;
        }
    }
    private bool CheckUserName()
    {
        if (string.IsNullOrEmpty(nameField.text))
        {
            Debug.Log("Please Enter User Name");
            nameFeedbackTxt.text = "Please Enter User Name";
            return false;
        }
        //else if (nameField.text.Length < 2 || nameField.text.Length > 12)
        //{
        //    nameFeedbackTxt.text = "Name must be greater than 2 characters, and be 12 or less characters.";
        //    return false;
        //}
        else
        {
            nameFeedbackTxt.text = "";
            return true;
        }
    }
    private bool CheckEmail()
    {
        if (string.IsNullOrEmpty(emailField.text))
        {
            Debug.Log("Please Enter Email");
            emailFeedbackTxt.text = "Please Enter Email";
            return false;
        }
        else if (validate.ValidateEmail(emailField.text) == Validate.ErrorCode.INVALID)
        {
            Debug.Log("Please Enter Correct Email Adress");
            emailFeedbackTxt.text = "Please Enter Correct Email Adress";
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
        if (string.IsNullOrEmpty(passwordField.text))
        {
            Debug.Log("Please Enter Password");
            passwordFeedbackTxt.text = "Please Enter Password";
            return false;
        }
        else if (validate.ValidatePassword(passwordField.text) == Validate.ErrorCode.INVALID)
        {
            passwordFeedbackTxt.text = "Please Enter the Password with 6 charcater,special charcater & number";
            return false;
        }
        else
        {
            passwordFeedbackTxt.text = "Password is valid";
            return true;
        }
    }
    private bool CheckConfirmPassword()
    {
        if (string.IsNullOrEmpty(confirmPasswordField.text))
        {
            confirmPasswordFeedbackTxt.text = "Please Enter Confirm Password";
            return false;
        }
        else if (!passwordField.text.Contains(confirmPasswordField.text))
        {
            confirmPasswordFeedbackTxt.text = "Password And Confirm Password should be same";
            return false;
        }
        else
        {
            confirmPasswordFeedbackTxt.text = "Confirm Password is Valid";
            return true;
        }
    }
    private void RefreshText()
    {
        try
        {
            nameField.text = "";
            emailField.text = "";
            passwordField.text = "";
            confirmPasswordField.text = "";
            nameFeedbackTxt.text = "";
            emailFeedbackTxt.text = "";
            passwordFeedbackTxt.text = "";
            confirmPasswordFeedbackTxt.text = "";
            organizationFeedbackTxt.text = "";
            organizationField.text = "";
        } 
        catch (Exception e)
        {
            Debug.LogError("The text fields game objects weren't set");
        }
        
    }
    #endregion

    #region Coroutine
    IEnumerator HideFeedbackText()
    {
        StopCoroutine("HideFeedbackText");
        yield return new WaitForSeconds(2.0f);
        //feedbackTxt.text = "";
    }
    #endregion
}
