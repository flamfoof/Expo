using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class TempCheckPassword : MonoBehaviour
{
    string password1 = BaseStrings.password1;
    string password2 = BaseStrings.password2;

    [SerializeField] private InputField passwordInput;
    public UIControlsDemo UIControls;

    private void Awake()
    {
        UIControls = GameObject.FindObjectOfType<UIControlsDemo>();
    }

    // Start is called before the first frame update
    public void CheckPassword()
    {
        if(passwordInput.text == password1 || passwordInput.text == password2)
        {
            SessionHandler.instance.passAdress = passwordInput.text;
            UIControls.characterSelect.SetActive(true);
            UIControls.roleSelect.SetActive(false);
        }
    }

    public void CheckForPresenter(bool isPresenter)
    {
        SessionHandler.instance.SetPresenter(isPresenter);
    }

    public void CheckForStaff(bool isStaff)
    {
        SessionHandler.instance.SetStaff(isStaff);
    }
}
