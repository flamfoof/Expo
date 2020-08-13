using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SelectAvatar : MonoBehaviour
{
    [SerializeField] private Button bodyPreviousBttn;
    [SerializeField] private Button bodyNextBttn;
    [SerializeField] private Button headPreviousBttn;
    [SerializeField] private Button headNextBttn;
    private int currentHeadCharacterCount;
    private int currentBodyCharacterCount;
    private int femaleHeadCharacterCount;
    private int femaleBodyCharacterCount;
    private int maleHeadCharacterCount;
    private int maleBodyCharacterCount;
    private UIControlsDemo UIControls;
    public AssignPlayerAvatar AssignAvatar;
    AssignPlayerAvatar selectedAvatarType;

    public scr_Selector maleCharacteristics;
    public scr_Selector_Female femaleCharacteristics;

    string avatarGender = "male";

    private void Awake()
    {
        UIControls = GameObject.FindObjectOfType<UIControlsDemo>();
    }
    private void Start()
    {
        currentHeadCharacterCount = maleHeadCharacterCount = maleCharacteristics.Heads.Count;
        currentBodyCharacterCount = maleHeadCharacterCount= maleCharacteristics.Suits.Count;

        //getting character variation count before hiding female gameobject
        femaleHeadCharacterCount = femaleCharacteristics.SkinSuit.Count;
        femaleBodyCharacterCount = femaleCharacteristics.Suit.Count;

        femaleCharacteristics.gameObject.SetActive(false);

    }

    public void SwitchAvatarGender(string avatarName)
    {
        avatarGender = avatarName;
        if (avatarGender == "male")
        {
            currentBodyCharacterCount = maleHeadCharacterCount;
            currentHeadCharacterCount = maleHeadCharacterCount;

            maleCharacteristics.gameObject.SetActive(true);
            femaleCharacteristics.gameObject.SetActive(false);
        }
        else
        {
            currentBodyCharacterCount = femaleHeadCharacterCount;
            currentHeadCharacterCount = femaleHeadCharacterCount;

            femaleCharacteristics.gameObject.SetActive(true);
            maleCharacteristics.gameObject.SetActive(false);
       }
        //pressing previous button to reset UI

        PreviousButtonClick(avatarGender);
    }

    private void OnEnable()
    {
        EnableCharHead(AssignAvatar.headIndex);
        EnableCharBody(AssignAvatar.bodyIndex);

        if (AssignAvatar.headIndex == 0)
        {
            headPreviousBttn.interactable = false;
            bodyPreviousBttn.interactable = false;
        }
    }

    private void DeactivateAllCharacter()
    {
        for (int i = 0; i < UIControls.selectAvatarObj.Length; i++)
        {
            UIControls.selectAvatarObj[i].SetActive(false);
        }
    }

    private void EnableCharBody(int index)
    {
        if (avatarGender == "male")
        {
            maleCharacteristics.pickOneSuit(index);
        }
        else
        {
            femaleCharacteristics.pickSuit(index);
        }
    }

    private void EnableCharHead(int index)
    {
        if (avatarGender == "male")
        {
            maleCharacteristics.PickOneHead(index);

        }
        else
        {
            femaleCharacteristics.pickSkin(index);
        }
    }

    public void PreviousButtonClick(string characteristic)
    {

        print("Setting up head #" + AssignAvatar.headIndex + "/" + currentHeadCharacterCount);

        if (characteristic == "head")
        {
            if (AssignAvatar.headIndex <= 1)
            {
                headPreviousBttn.interactable = false;
            }
            else
            {
                AssignAvatar.headIndex--;

                headPreviousBttn.interactable = true;
                headNextBttn.interactable = true;
            }
            EnableCharHead(AssignAvatar.headIndex);
        }
        else if (characteristic == "body")
        {
            if (AssignAvatar.bodyIndex <= 0)
            {
                bodyPreviousBttn.interactable = false;
            }
            else
            {
                AssignAvatar.bodyIndex--;

                bodyPreviousBttn.interactable = true;
                bodyNextBttn.interactable = true;
            }
            EnableCharBody(AssignAvatar.bodyIndex);
        }
    }

    public void NextButtonClick(string characteristic)
    {
        if (characteristic == "head")
        {

            AssignAvatar.headIndex++;
            print("Setting up head #" + AssignAvatar.headIndex + "/" + currentHeadCharacterCount);

            if (AssignAvatar.headIndex >= currentHeadCharacterCount-1)
            {
                headNextBttn.interactable = false;
                AssignAvatar.headIndex = currentHeadCharacterCount - 1;
            }
            else
            {
                headPreviousBttn.interactable = true;
                headNextBttn.interactable = true;
            }
            EnableCharHead(AssignAvatar.headIndex);
        }
        else if (characteristic == "body")
        {
            AssignAvatar.bodyIndex++;
            print("Setting up head #" + AssignAvatar.bodyIndex + "/" + currentBodyCharacterCount);

            if (AssignAvatar.bodyIndex >= currentBodyCharacterCount - 1)
            {
                bodyNextBttn.interactable = false;
            }
            else
            {
                bodyPreviousBttn.interactable = true;
                bodyNextBttn.interactable = true;
            }
            EnableCharBody(AssignAvatar.bodyIndex);
        }
    }
}
