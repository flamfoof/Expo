using System.Collections;
using UnityEngine;

public class TutorialBehavior : MonoBehaviour
{
    public GameObject[] tutorialScreens;
    bool waitingforscreen = false;
    int currentScreenNum = 0;
    GameObject currentScreen;

    void Start()
    {
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
            if (Input.GetMouseButton(1) && currentScreen.name == "rightClickTutorialScreen")
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
        yield return new WaitForSeconds(1f);
        waitingforscreen = false;
        currentScreen = tutorialScreens[currentScreenNum];
        currentScreen.SetActive(true);

    }
}
