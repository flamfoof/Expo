using System.Collections;
using UnityEngine;
using Photon.Pun;
using System;

public class TutorialBehavior : MonoBehaviour
{
    public GameObject[] tutorialScreens;
    public GameObject player;
    bool waitingforscreen = false;
    int currentScreenNum = 0;
    GameObject currentScreen;

    void Start()
    {
        if (!PlayerPrefs.HasKey("tutorialDone"))
        {
            PlayerPrefs.SetInt("tutorialDone", 0);
        }

        if(PlayerPrefs.GetInt("tutorialDone") == 1)
        {
            gameObject.SetActive(false);
        }

        if (IgniteGameManager.localPlayer != player)
        {
            gameObject.SetActive(false);
        }

        currentScreen = tutorialScreens[currentScreenNum];
    }

    // Update is called once per frame
    void Update()
    {
        if (!waitingforscreen && PlayerPrefs.GetInt("tutorialDone") == 0)
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
                tutorialScreens[currentScreenNum].SetActive(false);
            }
            if (Input.GetKeyDown(KeyCode.Return) && currentScreen.name == "chatTutorialScreen")
            {
                PlayerPrefs.SetInt("tutorialDone", 1);
                waitingforscreen = true;
                Destroy(gameObject);
            }
        }
    }
    public void MenuTutorialDone()
    {
        waitingforscreen = true;
        NextTutorialScreen();
    }

    private void WaitForClick()
    {
        throw new NotImplementedException();
    }

    void NextTutorialScreen()
    {
        tutorialScreens[currentScreenNum].SetActive(false);
        currentScreenNum++;

        StartCoroutine("WaitForNewUI");
    }

    IEnumerator WaitForNewUI()
    {
        yield return new WaitForSeconds(.7f);
        waitingforscreen = false;
        currentScreen = tutorialScreens[currentScreenNum];
        currentScreen.SetActive(true);
    }
}
