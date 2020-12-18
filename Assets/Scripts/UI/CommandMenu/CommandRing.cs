using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class CommandRing : MonoBehaviour
{
    public GameObject commandCenter;
    public GameObject[] commands;
    public GameObject cancel;
    public IgniteGameManager gameManager;
    public Button[] startEndStream_Btns;
    public UserActions player;
    public Quaternion playerHeadRotation;
    public Vector3 worldPos;
    public Vector3 mousePos;
    public Vector3 offset;
    public bool isEnable;
    public bool isMobile = false;
    public float cancelDistance = 0.25f;
    public bool isMainCommand = true;
    public Button dummyButton;

    //tutorial
    public GameObject[] tutorialElements;


    public float visualDistance = 5.0f;

    void Start()
    {
        gameManager = IgniteGameManager.IgniteInstance;
        if (IgniteGameManager.localPlayer != null)
        {
            player = IgniteGameManager.localPlayer.GetComponent<UserActions>();
        }

        this.gameObject.SetActive(false);
        
#if UNITY_IOS || UNITY_ANDROID
        isMobile = true;
#endif
    }

    void Update()
    {
        if (isEnable && isMainCommand && !isMobile)
        {
            Vector3 diff;
            transform.LookAt(player.playerActionRay.transform);
            mousePos = (player.playerActionRay.transform.forward * visualDistance) + player.playerActionRay.transform.position;
            diff = worldPos - mousePos;
            transform.position = player.playerActionRay.transform.position + offset;

            //TODO:
            //properties to set for auto select with Direction input
            RaycastWorldUI();
            /*
            if(DistanceFromCenter() < cancelDistance)
            {
                cancel.GetComponent<Button>().Select();
            } else 
            {
                //dummyButton.OnSelect(null);
            }*/
            //V1.x * V2.x  +  V1.y * V2.y  +  V1.z * V2.z
            /*
            Debug.Log((float)Vector3.Dot(mousePos.normalized, Input.mousePosition.normalized));
            Debug.Log("Mouse Pos: " + mousePosNorm);
            Debug.Log("New Mouse Pos: " + inputMouseNorm);*/
        }
        else if (!isMainCommand)
        {
            return;
        }
    }

    void OnEnable()
    {
        if (PlayerPrefs.GetInt("tutorialDone") == 0)
        {
            StartMenuTutorial();
        }

        if (!player && IgniteGameManager.localPlayer != null)
        {
            player = IgniteGameManager.localPlayer.GetComponent<UserActions>();

        }
        if (isMainCommand && player)
        {
            transform.position = (player.playerActionRay.transform.forward * visualDistance) + player.playerActionRay.transform.position;
            offset = (player.playerActionRay.transform.forward * visualDistance);
            worldPos = (player.playerActionRay.transform.forward * visualDistance) + player.playerActionRay.transform.position;
            playerHeadRotation = player.playerActionRay.transform.rotation;
            isEnable = true;
        }
    }

    void Ondisable()
    {
        isEnable = false;
    }

    void FindDot()
    {

    }

    float DistanceFromCenter()
    {
        float distance = Vector3.Distance(transform.position, mousePos);
        return distance;
    }

    void RaycastWorldUI()
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current);
        pointerData.position = Input.mousePosition;

#if UNITY_WEBGL
        //This is because the locked cursor is not always center on WebGL
        if(Cursor.lockState == CursorLockMode.Locked)
        {
            Vector2 screenXY = new Vector2(Screen.width/2, Screen.height/2);
            pointerData.position = screenXY;
        }
        
#endif

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        /*
        if (results.Count > 0) {
                        //WorldUI is my layer name
            if (results[0].gameObject.layer == LayerMask.NameToLayer("Interactables")){ 
                string dbg = "Root Element: {0} \n GrandChild Element: {1}";
                Debug.Log(string.Format(dbg, results[results.Count-1].gameObject.name,results[0].gameObject.name));
                //Debug.Log("Root Element: "+results[results.Count-1].gameObject.name);
                //Debug.Log("GrandChild Element: "+results[0].gameObject.name);
                results.Clear();
            }
        }     */
        if (results.Count > 0)
        {
            if (results[0].gameObject.layer == LayerMask.NameToLayer("Interactables"))
            {
                if (results[results.Count - 1].gameObject.GetComponent<CommandButton>())
                {
                    results[results.Count - 1].gameObject.GetComponent<CommandButton>().Hover();
                }
                else
                {
                    dummyButton.Select();
                }
            }


        }
    }

    public void EnableStreamBtns(bool status)
    {
        for (int i = 0; i < startEndStream_Btns.Length; i++)
        {
            startEndStream_Btns[i].gameObject.SetActive(status);
        }
    }

    public void StartMenuTutorial()
    {
        foreach (GameObject g in tutorialElements)
        {
            g.SetActive(true);
        }
    }

    public void FinishMenuTutorial()
    {
        if (PlayerPrefs.GetInt("tutorialDone") == 0)
        {
            foreach (GameObject g in tutorialElements)
            {
                g.SetActive(false);
            }
            GameObject.FindObjectOfType<TutorialBehavior>().MenuTutorialDone();
        }
    }

    public void RaiseHand()
    {
        //print(player.handRaise.name);
        print("HAND RAISE: " + player.isHandRaised);
        if(player.isHandRaised)
        {
            player.isHandRaised = false;
            player.handRaise.SetActive(false);
        }
        else
        {
            player.isHandRaised = true;
            player.handRaise.SetActive(true);
        }
        player.HandRaiseClicked();
    }

}
