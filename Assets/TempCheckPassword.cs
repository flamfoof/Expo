using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TempCheckPassword : MonoBehaviour
{
    string password = BaseStrings.password;
    [SerializeField] private InputField passwordInput;
    public UIControlsDemo UIControls;

    private void Awake()
    {
        UIControls = GameObject.FindObjectOfType<UIControlsDemo>();
    }

    // Start is called before the first frame update
    public void CheckPassword()
    {
        if(passwordInput.text == password)
        {
            UIControls.characterSelect.SetActive(true);
            UIControls.roleSelect.SetActive(false);
        }
    }

    public void CheckForPresenter(bool isPresenter)
    {
        SessionHandler.instance.SetPresenter(isPresenter);
    }
}
