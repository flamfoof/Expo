using System.Collections;
using UnityEngine;
using Photon.Pun;

public class TutorialBehavior : MonoBehaviour
{
    public GameObject[] tutorialScreens;
    public GameObject player;
    bool waitingforscreen = false;
    int currentScreenNum = 0;
    GameObject currentScreen;

    void Start()
    {
        
        if(IgniteGameManager.localPlayer != player)
        {
            gameObject.SetActive(false);
        }
        currentScreen = tutorialScreens[currentScreenNum];
        
    }
    // Update is called once per frame
    void Update()
    {

        if (!waitingforscreen)
        {
            if (Input.GetKeyDown(KeyCode.W) && currentScreen.name == "wTutorialScreen")
            {
                waitingforscreen = true;
                NextTutorialScreen();
            }
            if (Input.GetKeyDown(KeyCode.UpArrow) && currentScreen.name == "wTutorialScreen")
            {
                waitingforscreen = true;
                NextTutorialScreen();
            }
            if (Input.GetMouseButton(0) && currentScreen.name == "rightClickTutorialScreen")
            {
                waitingforscreen = true;
                NextTutorialScreen();
            }
            if (Input.GetKeyDown(KeyCode.Return) && currentScreen.name == "chatTutorialScreen")
            {
                waitingforscreen = true;
                Destroy(gameObject);
            }
        }
    }

    void NextTutorialScreen()
    {
        tutorialScreens[currentScreenNum].SetActive(false);
        currentScreenNum++;

        StartCoroutine("WaitForNewUI");
    }

    IEnumerator WaitForNewUI()
    {
        yield return new WaitForSeconds(1.5f);
        waitingforscreen = false;
        currentScreen = tutorialScreens[currentScreenNum];
        currentScreen.SetActive(true);

    }
}
