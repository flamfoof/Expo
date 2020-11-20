using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatInteractableToggle : MonoBehaviour
{
    CanvasGroup canvasGroup;

    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(canvasGroup.alpha < 5.0f)
        {
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        } else {
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }
    }
}
