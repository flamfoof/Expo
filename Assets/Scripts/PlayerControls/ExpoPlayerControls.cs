using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

public class ExpoPlayerControls : MonoBehaviour
{
    
    private PlayerInput playerInput;
    private InputAction actionLook;
    private InputAction actionMove;
    private InputAction actionPrimary;
    private InputAction actionSecondary;
    private InputAction.CallbackContext context;

    private bool isButtonHeld;

    private void Awake() {
        if (playerInput == null)
        {
                 playerInput = GetComponent<PlayerInput>();
                 actionPrimary = playerInput.actions["Primary"];
                 actionSecondary = playerInput.actions["Secondary"];
                 actionLook = playerInput.actions["Look"];
                 actionMove = playerInput.actions["Move"];
        }
    }

    private void OnEnable() {
        actionPrimary.started += context => Interact(context);
        actionPrimary.performed += context => Interact(context);
        actionPrimary.canceled += context => Interact(context);
        actionPrimary.canceled += context => Interact(context);
        actionPrimary.canceled += context => Interact(context);
        actionPrimary.canceled += context => Interact(context);
    }


    void Update()
    {
        
    }

    private void Interact(InputAction.CallbackContext ctx)
    {
        switch (context.phase)
        {            
            case InputActionPhase.Performed:
                if (context.interaction is SlowTapInteraction)
                {
                    //StartCoroutine(HoldButtonPress((int)(context.duration)));
                }
                else
                {
                    //If not held, then do this function
                    //something()
                }
                isButtonHeld = false;
                break;

            case InputActionPhase.Started:
                if (context.interaction is SlowTapInteraction)
                    isButtonHeld = true;
                break;

            case InputActionPhase.Canceled:
                isButtonHeld = false;
                break;
        }
    }

    private void CancelButton(InputAction.CallbackContext ctx)
    {
        switch (context.phase)
        {            
            case InputActionPhase.Performed:
                if (context.interaction is SlowTapInteraction)
                {
                    //StartCoroutine(HoldButtonPress((int)(context.duration)));
                }
                else
                {
                    //Something();
                }
                isButtonHeld = false;
                break;

            case InputActionPhase.Started:
                if (context.interaction is SlowTapInteraction)
                    isButtonHeld = true;
                break;

            case InputActionPhase.Canceled:
                isButtonHeld = false;
                break;
        }
    }

    private IEnumerator HoldButtonPress()
    {
        //Do something - Activate the interactable
        yield return new WaitForSeconds(0.1f);
    }
}
