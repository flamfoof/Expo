using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ISimpleLink : Interactables
{
    public string linkURL;

    public override void Perform(InputActionPhase phase)
    {

        Application.OpenURL(linkURL);
    }
}
