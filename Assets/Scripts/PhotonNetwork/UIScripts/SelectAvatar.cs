using UnityEngine;
using UnityEngine.UI;

public class SelectAvatar : MonoBehaviour
{
    [SerializeField] private Button bodyPreviousBttn;
    [SerializeField] private Button bodyNextBttn;
    [SerializeField] private Button headPreviousBttn;
    [SerializeField] private Button headNextBttn;
    private int totalHeadCharacterCount;
    private int totalBodyCharacterCount;
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
        totalHeadCharacterCount = maleHeadCharacterCount = maleCharacteristics.Heads.Count;
        totalBodyCharacterCount = maleHeadCharacterCount = maleCharacteristics.Suits.Count;
        print("male body count = " + maleCharacteristics.Heads.Count);
        print("male body count = " + maleCharacteristics.Heads.Count);
        //getting character variation count before hiding female gameobject
        femaleHeadCharacterCount = femaleCharacteristics.SkinSuit.Count;
        femaleBodyCharacterCount = femaleCharacteristics.Suit.Count;
        print("female body count = " + femaleBodyCharacterCount);
        print("female body count = " + femaleHeadCharacterCount);

        //not working, make move it down position wise and then move it back up after it fills all the characters and deactivate it before moving it up.
        femaleCharacteristics.gameObject.SetActive(false);
    }

    public void SwitchAvatarGender(string avatarName)
    {
        avatarGender = avatarName;

        if (avatarGender == "male")
        {
            totalBodyCharacterCount = maleHeadCharacterCount;
            totalHeadCharacterCount = maleHeadCharacterCount;
            AssignAvatar.Gender = GenderList.genders.Male1;
            

            maleCharacteristics.gameObject.SetActive(true);
            femaleCharacteristics.gameObject.SetActive(false);
        }
        else
        {
            totalBodyCharacterCount = femaleHeadCharacterCount;
            totalHeadCharacterCount = femaleHeadCharacterCount;
            AssignAvatar.Gender = GenderList.genders.Female;

            femaleCharacteristics.gameObject.SetActive(true);
            maleCharacteristics.gameObject.SetActive(false);
        }

        //reset index
        AssignAvatar.headIndex = 1;
        AssignAvatar.bodyIndex = 1;
        EnableCharHead(AssignAvatar.headIndex);
        EnableCharBody(AssignAvatar.bodyIndex);
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
            print("Current asset: " + AssignAvatar.headIndex + "/" + totalHeadCharacterCount);

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
            print("Current asset: " + AssignAvatar.bodyIndex + "/" + totalBodyCharacterCount);

            EnableCharBody(AssignAvatar.bodyIndex);
        }
    }

    public void NextButtonClick(string characteristic)
    {
        if (characteristic == "head")
        {
            AssignAvatar.headIndex++;

            if (AssignAvatar.headIndex >= totalHeadCharacterCount - 1)
            {
                headNextBttn.interactable = false;
                AssignAvatar.headIndex = totalHeadCharacterCount - 1;
            }
            else
            {
                headPreviousBttn.interactable = true;
                headNextBttn.interactable = true;
            }
            print("Current asset: " + AssignAvatar.headIndex + "/" + totalHeadCharacterCount);

            EnableCharHead(AssignAvatar.headIndex);
        }
        else if (characteristic == "body")
        {
            AssignAvatar.bodyIndex++;

            if (AssignAvatar.bodyIndex >= totalBodyCharacterCount - 1)
            {
                bodyNextBttn.interactable = false;
            }
            else
            {
                bodyPreviousBttn.interactable = true;
                bodyNextBttn.interactable = true;
            }
            print("Current asset: " + AssignAvatar.bodyIndex + "/" + totalBodyCharacterCount);

            EnableCharBody(AssignAvatar.bodyIndex);
        }
    }
}
