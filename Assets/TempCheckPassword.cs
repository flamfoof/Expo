using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TempCheckPassword : MonoBehaviour
{
    string password = "atrium2020";
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

    // Update is called once per frame
    void Update()
    {
        
    }
}
