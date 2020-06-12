using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

public class ExpoPlayerControls : MonoBehaviour
{
    //All movement, including camera, is in FirstPersionAIO.cs
    //
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

                 //these movement inputs will be referenced from the FirstPersonAIO.cs 
                 actionLook = playerInput.actions["Look"];
                 actionMove = playerInput.actions["Move"];
        }

        //binds these buttons to the functions
        //i.e. the primary action button will activate the Interact function
        actionPrimary.started += context => Interact(context);
        actionPrimary.performed += context => Interact(context);
        actionPrimary.canceled += context => Interact(context);
        actionSecondary.started += context => CancelButton(context);
        actionSecondary.performed += context => CancelButton(context);
        actionSecondary.canceled += context => CancelButton(context);
    }

    private void OnEnable() {
        actionPrimary.Enable();
        actionSecondary.Enable();
        actionLook.Enable();
        actionMove.Enable();
    }

    private void OnDisable() {
        actionPrimary.Disable();
        actionSecondary.Disable();
        actionLook.Disable();
        actionMove.Disable();
    }


    void Update()
    {
        // Fail safe, you don't want them losing controls for no reason
        if (playerInput == null)
        {
                 playerInput = GetComponent<PlayerInput>();
                 actionPrimary = playerInput.actions["Primary"];
                 actionSecondary = playerInput.actions["Secondary"];

                 //these movement inputs will be referenced from the FirstPersonAIO.cs 
                 actionLook = playerInput.actions["Look"];
                 actionMove = playerInput.actions["Move"];
        }

    }

    private void Interact(InputAction.CallbackContext ctx)
    {
        switch (context.phase)
        {            
            // For when button is held.
            // Time held to perform is managed by the ExpoControls in PlayerInputs folder
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
            // Checks if button has been pressed
            case InputActionPhase.Started:
                if (context.interaction is SlowTapInteraction)
                    isButtonHeld = true;
                break;

            // Checks if button has been let go before the held time (before it's fully performed)
            case InputActionPhase.Canceled:
                isButtonHeld = false;
                Debug.Log("Interact button pressed");
                if(false)
                {

                }
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

    public PlayerInput GetPlayerInput()
    {
        return this.playerInput;
    }
    
    public InputAction GetActionMove()
    {
        return this.actionMove;
    }
    
    public InputAction GetActionLook()
    {
        return this.actionLook;
    }
}
