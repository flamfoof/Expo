using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class UserActions : MonoBehaviourPunCallbacks, IPunObservable
{
    //All movement, including camera, is in FirstPersionAIO.cs
    private IgniteGameManager gameManager;
    private PlayerInput playerInput;
    private InputAction actionLook;
    private InputAction actionMove;
    private InputAction actionPrimary;
    private InputAction actionSecondary;
    private InputAction actionSprint;
    private InputAction.CallbackContext context;
    private FirstPersonAIO playerController;
    private Vector3 realPosition;
    private Quaternion realRotation;
    
    public Camera camera;
    public InteractableRayIdentifier playerActionRay;
    public Animator anim;
    public TextMesh playerName;

    GameObject bodyOrigin;
    public bool disableServer = false;

    public float emoteForce = 20.0f;
    public int emoteAmount = 10;

    private bool isButtonHeld;

    private void Awake() {
        AttachControlsReference();
        playerController = GetComponent<FirstPersonAIO>();
        gameManager = GameObject.FindObjectOfType<IgniteGameManager>();

        //binds these buttons to the functions
        //i.e. the primary action button will activate the Interact function
        // Started - When button is pushed
        // Performed - When the button has been pushed and held for the corresponding amount of 
        //             time in the controller configs
        // Canceled - When button has been let go before it has been performed^
        actionPrimary.started += context => Interact(context);
        actionPrimary.performed += context => Interact(context);
        actionPrimary.canceled += context => Interact(context);
        actionSecondary.started += context => CancelButton(context);
        actionSecondary.performed += context => CancelButton(context);
        actionSecondary.canceled += context => CancelButton(context);
        actionSprint.started += context => SprintButton(context);        
        actionSprint.canceled += context => SprintButton(context);
    }

    private void Start() {
        //Debug.Log("Attaching anim");
        Invoke("AttachAnim", 1.0f);
        IgniteGameManager.IgniteInstance.RefreshOnPlayerSpawn();
        gameManager.SetParent(this.transform, gameManager.voiceManager.listener.transform);
    }

    private void OnEnable() {
        actionPrimary.Enable();
        actionSecondary.Enable();
        actionLook.Enable();
        actionMove.Enable();
        actionSprint.Enable();
    }

    private void OnDisable() {
        actionPrimary.Disable();
        actionSecondary.Disable();
        actionLook.Disable();
        actionMove.Disable();
        actionSprint.Disable();
    }


    void Update()
    {
        AttachControlsReference();

        //todo: disable if in VR
        if(anim)
        {
            UpdateAnim();
        }
        if(!disableServer)
        {
            if(photonView.IsMine)
            {

            } else {
                transform.position = Vector3.Lerp(transform.position, realPosition, .1f);
                transform.rotation = Quaternion.Lerp(transform.rotation, realRotation, .1f);
            }
        }

    }

    private void AttachControlsReference()
    {
        // Fail safe, you don't want them losing controls for no reason
        if (playerInput == null)
        {
            playerInput = GetComponent<PlayerInput>();
            actionPrimary = playerInput.actions["Primary"];
            actionSecondary = playerInput.actions["Secondary"];

            //some of these inputs will be referenced from the FirstPersonAIO.cs 
            actionLook = playerInput.actions["Look"];
            actionMove = playerInput.actions["Move"];
            actionSprint = playerInput.actions["Sprint"];
                 
        }
    }

    private void Interact(InputAction.CallbackContext ctx)
    {               
        //Debug.Log(ctx.phase);
        switch (ctx.phase)
        {                        
            // For when button is held.
            // Time held to perform is managed by the ExpoControls in PlayerInputs folder
            case InputActionPhase.Performed:
                if (ctx.interaction is SlowTapInteraction)
                {
                    //StartCoroutine(HoldButtonPress((int)(context.duration)));  
                    playerActionRay.UseInteractable(ctx.phase);                  
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
                if (ctx.interaction is SlowTapInteraction)
                {
                    isButtonHeld = true;
                    playerActionRay.UseInteractable(ctx.phase);                      
                
                }
                break;

            // Checks if button has been let go before the held time (before it's fully performed)
            case InputActionPhase.Canceled:
                if (ctx.interaction is SlowTapInteraction)
                {
                    isButtonHeld = false;
                    playerActionRay.UseInteractable(ctx.phase);   
                }
                
                //Debug.Log("Interact button pressed");
                
                break;
        }
    }

    private void CancelButton(InputAction.CallbackContext ctx)
    {
        switch (ctx.phase)
        {            
            case InputActionPhase.Performed:
                if (ctx.interaction is SlowTapInteraction)
                {
                    //StartCoroutine(HoldButtonPress((int)(ctx.duration)));
                }
                else
                {
                    //Something();
                }
                isButtonHeld = false;
                break;

            case InputActionPhase.Started:
                if (ctx.interaction is SlowTapInteraction)
                    isButtonHeld = true;
                
                Emote();
                break;

            case InputActionPhase.Canceled:
                isButtonHeld = false;

                break;
        }
    }

    private void SprintButton(InputAction.CallbackContext ctx)
    {
        switch (ctx.phase)
        {            
            case InputActionPhase.Performed:
               // Debug.Log("Startiing");
                break;

            case InputActionPhase.Started:
                GetComponent<FirstPersonAIO>().sprintKey = true;
                //Debug.Log("sprinting");
                break;

            case InputActionPhase.Canceled:
                GetComponent<FirstPersonAIO>().sprintKey = false;
                break;
        }
    }

    public void Emote()
    {
        Debug.Log("emoted");
        Vector3 randPos;
        float x, y, z;
        Debug.Log("Spawned them");
        for(int i = 0; i < emoteAmount; i++)
        {
            x = transform.position.x + Random.Range(-5.0f, 5.0f);
            y = transform.position.y + Random.Range(-5.0f, 5.0f);
            z = transform.position.z + Random.Range(-5.0f, 5.0f);
            randPos = new Vector3(x, y, z);
            PhotonNetwork.Instantiate(EmoteList.emotePath + GetComponent<EmoteList>().emotesList[Random.Range(0, 1)].name, randPos, Quaternion.identity);
            //Instantiate(GetComponent<EmoteList>().emotesList[0], randPos, Quaternion.identity);
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

    public void AttachAnim()
    {
        anim = GetComponent<AttachAvatar>().avatarBodyLocation.GetComponent<Animator>();
        if(anim)
        {
            //Debug.Log("Found the thing: " + anim.gameObject.name);
        }
    }

    public void UpdateAnim()
    {
        //add jump and other interactable animations here

        //Walk Conditions: speed > 1.0f
        anim.SetFloat("speed", playerController.groundVelocity);        
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            if(anim)
                stream.SendNext(anim.GetFloat("speed"));            
        } else {
            realPosition = (Vector3)stream.ReceiveNext();
            realRotation = (Quaternion)stream.ReceiveNext();
            if(anim)
            {            
                anim.SetFloat("speed", (float)stream.ReceiveNext());
            }
                
        }
    }
}
