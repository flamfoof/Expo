using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SelectAvatar : MonoBehaviour
{
    [SerializeField] private Button bodyPreviousBttn;
    [SerializeField] private Button bodyNextBttn;
    [SerializeField] private Button headPreviousBttn;
    [SerializeField] private Button headNextBttn;
    private int headCharacterCount;
    private int bodyCharacterCount;
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
        headCharacterCount = maleCharacteristics.Heads.Count;
        bodyCharacterCount = maleCharacteristics.Suits.Count;
    }

    public void SwitchAvatarGender(string avatarName)
    {
        avatarGender = avatarName;
        if (avatarGender == "male")
        {
            bodyCharacterCount = maleCharacteristics.Suits.Count;
            headCharacterCount = maleCharacteristics.Heads.Count;

            maleCharacteristics.gameObject.SetActive(true);
            femaleCharacteristics.gameObject.SetActive(false);
        }
        else
        {
            femaleCharacteristics.gameObject.SetActive(true);
            maleCharacteristics.gameObject.SetActive(false);
            StartCoroutine("GetCharacterSize");
       }
        //pressing previous button to reset UI

        PreviousButtonClick(avatarGender);
    }

    IEnumerator GetCharacterSize()
    {
        yield return new WaitForSeconds(.1f);
        bodyCharacterCount = femaleCharacteristics.SkinTrousers.Count;
        headCharacterCount = femaleCharacteristics.SuitTrousers.Count;
        print("Setting char size ** = head " + headCharacterCount + " body " + bodyCharacterCount);

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
        print("head index " + AssignAvatar.headIndex);
        print("body index" + AssignAvatar.bodyIndex);

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
            print("Setting up head #" + AssignAvatar.headIndex + "/" + headCharacterCount);

            if (AssignAvatar.headIndex >= headCharacterCount-1)
            {
                headNextBttn.interactable = false;
                AssignAvatar.headIndex = headCharacterCount - 1;
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
            print("Setting up head #" + AssignAvatar.bodyIndex + "/" + bodyCharacterCount);

            if (AssignAvatar.bodyIndex >= bodyCharacterCount - 1)
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
