using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;

public class Interactables : MonoBehaviour
{
    private int timesUsed;
    public int TimesUsed {get{return this.timesUsed; } set{timesUsed = value;}}
    public GameObject[] assetParts;

    public bool canAttach = false;

    //override this function on the inherited functions
    public virtual void Perform(InputActionPhase phase)
    {
        Debug.Log("Default functions");
        switch(phase)
        {
            case InputActionPhase.Performed:
                Debug.Log("Default performed");
                break;
            case InputActionPhase.Started:
            Debug.Log("Default started");
                break;
            case InputActionPhase.Canceled:
            Debug.Log("Default cancelled");
                break;               
        }

    }

    public enum InteractableType
    {
        Link,
        Raffle,
        Guess,
        Minigame,
        Cosmetic,
        Other
    }
}
