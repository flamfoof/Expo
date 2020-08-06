using UnityEngine;
using UnityEngine.UI;

public class SelectAvatar : MonoBehaviour
{
    [SerializeField] private Button bodyPreviousBttn;
    [SerializeField] private Button bodyNextBttn;
    [SerializeField] private Button headPreviousBttn;
    [SerializeField] private Button headNextBttn;
    private int characterCount = 9;
    private UIControlsDemo UIControls;
    public AssignPlayerAvatar AssignAvatar;

    public scr_Selector maleCharacteristics;

    private void Awake()
    {
        UIControls = GameObject.FindObjectOfType<UIControlsDemo>();
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
        maleCharacteristics.pickOneSuit(index);
    }
    private void EnableCharHead(int index)
    {
        maleCharacteristics.PickOneHead(index);
    }

    public void PreviousButtonClick(string characteristic)
    {
        if (characteristic == "head")
        {
            AssignAvatar.headIndex--;

            if (AssignAvatar.headIndex <= 0)
            {
                headPreviousBttn.interactable = false;
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
            AssignAvatar.bodyIndex--;
            if (AssignAvatar.bodyIndex <= 0)
            {
                bodyPreviousBttn.interactable = false;
            }
            else
            {
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
            print("Setting up head #" + AssignAvatar.headIndex);

            if (AssignAvatar.headIndex >= characterCount)
            {
                headNextBttn.interactable = false;
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
            if (AssignAvatar.bodyIndex >= characterCount)
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
