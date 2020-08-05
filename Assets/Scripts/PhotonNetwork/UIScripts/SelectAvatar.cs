using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectAvatar : MonoBehaviour
{
    [SerializeField] private Button previousBttn;
    [SerializeField] private Button nextBttn;
    private int characterCount = 9;
    private UIControlsDemo UIControls;

    private void Awake()
    {
        UIControls = GameObject.FindObjectOfType<UIControlsDemo>();
    }
    private void OnEnable()
    {
        UIControls.selectedAvatarIndex = 0;
        EnableSpecificCharacter(UIControls.selectedAvatarIndex);
        if (UIControls.selectedAvatarIndex == 0)
        {
            previousBttn.interactable = false;
        }
    }
    private void DeactivateAllCharacter()
    {
        for(int i = 0;i<UIControls.selectAvatarObj.Length;i++)
        {
            UIControls.selectAvatarObj[i].SetActive(false);
        }
    }
    private void EnableSpecificCharacter(int index)
    {
        DeactivateAllCharacter();
        UIControls.selectAvatarObj[index].SetActive(true);
    }
    public void PreviousButtonClick()
    {
        UIControls.selectedAvatarIndex--;
        if (UIControls.selectedAvatarIndex <= 0)
        {
            previousBttn.interactable = false;
        }
        else
        {
            previousBttn.interactable = true;
            nextBttn.interactable = true;
        }
        EnableSpecificCharacter(UIControls.selectedAvatarIndex);
    }
    public void NextButtonClick()
    {
        UIControls.selectedAvatarIndex++;
        if (UIControls.selectedAvatarIndex >= characterCount)
        {
            nextBttn.interactable = false;
        }
        else
        {
            previousBttn.interactable = true;
            nextBttn.interactable = true;
        }
        EnableSpecificCharacter(UIControls.selectedAvatarIndex);
    }
}
