using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Photon.Pun;
using Photon.Realtime;
using Random = UnityEngine.Random;
using Hashtable = ExitGames.Client.Photon.Hashtable;

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
    private InputAction actionMenu;
    private InputAction actionChat;
    private InputAction.CallbackContext context;
    private FirstPersonAIO playerController;
    public GameObject playerUI;
    public GameObject commandUI;
    public CommunicationManager rtc;
    public bool isAppFocused = true;
    public bool isMenuOpen = false;
    public bool isChatOpen = false;
    public float chatFadeTimer = 3.0f;
    public bool isChatFading = false;
    public bool isCommandRingOpen = false;
    public bool canMove = true;
    public bool canLook = true;
    public int ping;
    private Vector3 realPosition;
    private Quaternion realRotation;
    public float lerpSpeed = 1.0f;
    private CanvasGroup infoCanvasGroup;

    public Camera camera;
    public InteractableRayIdentifier playerActionRay;
    public Animator anim;
    public TextMesh playerName;
    public TextMesh playerOrganization;
    public GameObject infoPopup; //For InfoPopUp
    private UIEffectsUtils uiEffects;
    public float sessionTimer = 0.0f;
    public float realSessionTimer = 0.0f;
    

    public float SessionTimer {
        get{
            if(photonView.IsMine)
            {
                return this.sessionTimer;
            } else {
                return this.realSessionTimer;
            }
        }
        set {
            if(photonView.IsMine)
            {
                this.sessionTimer = value;
            } else {
                this.realSessionTimer = value;
            }
        }
    }
    
    public List<string> objectsInteracted;

    GameObject bodyOrigin;
    public bool disableServer = false;

    public float emoteForce = 20.0f;
    public int emoteAmount = 10;

    private bool isButtonHeld;

    private void Awake() {
        AttachControlsReference();
        playerController = GetComponent<FirstPersonAIO>();
        gameManager = GameObject.FindObjectOfType<IgniteGameManager>();
        rtc = GameObject.FindObjectOfType<CommunicationManager>();
        //InvokeRepeating("UpdateLoginTime", 1.0f, 1.0f);

        //binds these buttons to the functions
        //i.e. the primary action button will activate the Interact function
        // Started - When button is pushed
        // Performed - When the button has been pushed and held for the corresponding amount of 
        //             time in the controller configs
        // Canceled - When button has been let go before it has been performed^
        actionPrimary.started += context => Interact(context);
        actionPrimary.performed += context => Interact(context);
        actionPrimary.canceled += context => Interact(context);
        actionSecondary.started += context => SecondaryButton(context);
        actionSecondary.performed += context => SecondaryButton(context);
        actionSecondary.canceled += context => SecondaryButton(context);
        actionSprint.started += context => SprintButton(context);        
        actionSprint.canceled += context => SprintButton(context);
        actionMenu.started += context => MenuButton(context);
        actionChat.started += context => ChatButton(context);
        //actionEmailBttn.started += context => EmailButton(context);
        //actionEmailBttn.canceled += context => EmailButton(context);
        infoCanvasGroup = GetComponent<CanvasGroup>();
        commandUI = gameManager.commandUI.gameObject;
        uiEffects = IgniteGameManager.IgniteInstance.GetComponent<UIEffectsUtils>();
    }

    private void Start() {
        //Debug.Log("Attaching anim");
        infoPopup.SetActive(false);   //Set the Popup inactive
        Invoke("AttachAnim", 1.0f);
        IgniteGameManager.IgniteInstance.RefreshOnPlayerSpawn();     
        IgniteGameManager.IgniteInstance.RefreshUniquePlayer();
        commandUI = gameManager.commandUI.gameObject;

        //cutting off webgl micrphone setting for now
        //if(photonView.IsMine)
            //gameManager.SetParent(this.transform, gameManager.voiceManager.listener.transform);
        
        //Announce();
        
    }

    /*
    public void Announce()
    {
        //mostly used for debugging microphone connection for voice recording in WebGL
        PhotonView pv = PhotonView.Get(this);
        //pv.RPC("AnnounceVoiceConnect", RpcTarget.All, PhotonNetwork.NickName, FrostweepGames.Plugins.Native.CustomMicrophone.IsRecording(FrostweepGames.Plugins.Native.CustomMicrophone.devices[0]));
    }

    [PunRPC]
    public void AnnounceVoiceConnect(string playerName, bool micState)
    {
        Debug.Log("Player " + playerName + " has started recording. Succeeded? - " + micState);
    }
*/

    private void OnEnable() {
        actionPrimary.Enable();
        actionSecondary.Enable();
        actionLook.Enable();
        actionMove.Enable();
        actionSprint.Enable();
        actionMenu.Enable();
        actionChat.Enable();
        //actionEmailBttn.Enable();
        
    }

    private void OnDisable() {
        actionPrimary.Disable();
        actionSecondary.Disable();
        actionLook.Disable();
        actionMove.Disable();
        actionSprint.Disable();
        actionChat.Disable();
        //actionEmailBttn.Disable();
    }

    void Update()
    {
        AttachControlsReference();

        if(photonView.IsMine)
            sessionTimer += Time.deltaTime;

        //todo: disable if in VR
        if(anim)
        {
            UpdateAnim();
        }
        if(!disableServer)
        {
            if(photonView.IsMine)
            {

            }
            else {
                transform.position = Vector3.Lerp(transform.position, realPosition, lerpSpeed);
                transform.rotation = Quaternion.Lerp(transform.rotation, realRotation, lerpSpeed);
                //Show the Popup if the hover on the player.
                if(playerActionRay.playerfocusedObject)
                {
                    infoCanvasGroup.alpha = 1.0f;
                    infoPopup.SetActive(true);
                }
                //Disable the Popup
                else
                {
                    if(infoPopup.activeInHierarchy)
                    {
                        StartCoroutine(AlphaFadeOut(infoCanvasGroup.alpha,0,3.0f));
                    }
                }
            }
        }
        ping = PhotonNetwork.GetPing();
    }

    //Fade Out the Info Popup Canvas 
    IEnumerator AlphaFadeOut(float start,float end,float duration)
    {
        StopCoroutine("AlphaFadeOut");
        float counter = 0f;
        while(counter < duration)
        {
            counter += Time.deltaTime;
            infoCanvasGroup.alpha = Mathf.Lerp(start,end,counter/duration);
            yield return null;
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
            actionMenu = playerInput.actions["Menu"];
            actionChat = playerInput.actions["Chat"];
        }
    }

    private void Interact(InputAction.CallbackContext ctx)
    {               
        //Debug.Log(ctx.phase);
        if(!isAppFocused || isCommandRingOpen)
        {
            return;
        }

        if(isMenuOpen)
        {
            return;
        }
        
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

    public void EmailButton(InputAction.CallbackContext ctx)
    {
        //After Click on the Info Button
        Application.OpenURL("mailto:" + PlayerPrefs.GetString("Email") + "?subject=" + "It was great meeting you today!" + "&body=" + "Sent from \n the We Ignite Platform");
    }

    private void SecondaryButton(InputAction.CallbackContext ctx)
    {
        if(isMenuOpen)
        {
            return;
        }

        switch (ctx.phase)
        {            
            case InputActionPhase.Performed:
                if (ctx.interaction is SlowTapInteraction)
                {                    
                    ActivateCommandRing();
                }
                else
                {
                    //Something();
                }
                isButtonHeld = false;
                break;

            case InputActionPhase.Started:
                if (ctx.interaction is SlowTapInteraction)
                {
                    OpenCommandRing(true);
                }
                isButtonHeld = true;
                //Emote();
                break;

            case InputActionPhase.Canceled:
                isButtonHeld = false;
                if (ctx.interaction is SlowTapInteraction)
                {                    
                    OpenCommandRing(false);
                }                
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

    private void MenuButton(InputAction.CallbackContext ctx)
    {
        switch (ctx.phase)
        {
            case InputActionPhase.Performed:                
                break;

            case InputActionPhase.Started:
                OpenMenu(!isMenuOpen);
                break;

            case InputActionPhase.Canceled:
                break;
        }
    }

    private void ChatButton(InputAction.CallbackContext ctx)
    {
        switch (ctx.phase)
        {
            case InputActionPhase.Performed:                
                break;

            case InputActionPhase.Started:
                OpenChat(!isChatOpen);
                break;

            case InputActionPhase.Canceled:
                break;
        }
    }

    public void OpenCommandRing(bool toggle)
    {
        if(toggle)
        {
            //UpdateControlLock(true, false);
            commandUI.SetActive(true);            
            isCommandRingOpen = toggle;
            //StartCoroutine(UnfocusApplicationCursor());
        } else {
            //UpdateControlLock(true, true);
            commandUI.SetActive(false);
            isCommandRingOpen = toggle;
            //StartCoroutine(RefocusApplicationCursor());
        }
    }

    public void ActivateCommandRing()
    {
        GameObject selectedButton = null;
        if(EventSystem.current.currentSelectedGameObject)
        {
            selectedButton = EventSystem.current.currentSelectedGameObject;
            if(selectedButton.GetComponent<CommandButton>())
                EventSystem.current.currentSelectedGameObject.GetComponent<Button>().onClick.Invoke();
        }
        OpenCommandRing(false);
    }

    public void OpenMenu(bool toggle)
    {
        if(toggle)
        {
            UpdateControlLock(false, false);
            StartCoroutine(UnfocusApplicationCursor());
            playerUI.SetActive(toggle);
            isMenuOpen = toggle;
            
        } else {
            UpdateControlLock(true, true);
            StartCoroutine(RefocusApplicationCursor());
            playerUI.SetActive(toggle);
            isMenuOpen = toggle;
        }  

        //close chat if menu is open
        if(isChatOpen)
        {
            rtc.webRTC.uMessageField.text = "";
            OpenChat(false);      
        }
            
    }

    public void OpenChat(bool toggle)
    {            
        if(toggle)
        {
            UpdateControlLock(false, true);
            StartCoroutine(UnfocusApplicationCursor());
            rtc.sendMessage.SetActive(toggle);
            rtc.webRTC.uMessageField.text = "";
            rtc.webRTC.uMessageField.Select();
            rtc.webRTC.uMessageField.ActivateInputField();
            if(isChatFading)
            {
                isChatFading = false;
            }
            StartCoroutine(FadeChat(true));
            isChatOpen = toggle;
        } else {            
            UpdateControlLock(true, true);
            StartCoroutine(RefocusApplicationCursor());
            if (rtc.webRTC.uMessageField.text != "") {
                rtc.webRTC.SendButtonPressed();
            }
            StartCoroutine(FadeChat(false));
            rtc.sendMessage.SetActive(toggle);
            isChatOpen = toggle;
        }
    }

    public IEnumerator FadeChat(bool toggle)
    {
        float timer = 0.0f;
        isChatFading = true;

        if(toggle == false)
        {
            while(timer < chatFadeTimer)
            {
                yield return new WaitForEndOfFrame();
                timer += Time.deltaTime;
                if(!isChatFading)
                    break;
            }
        }
        
        if(isChatFading)
        {
            if(toggle)
            {
                if(rtc.webRTC.receiveTxt.GetComponent<CanvasGroup>().alpha != 1)
                {
                    uiEffects.FadeInCanvasGroup(rtc.webRTC.receiveTxt.GetComponent<CanvasGroup>(), 2.0f);
                }
                    
            } else {
                if(!rtc.webRTC.lockChatVisibility)
                    uiEffects.FadeOutCanvasGroup(rtc.webRTC.receiveTxt.GetComponent<CanvasGroup>(), 1.0f);
            }  
        }
         
        isChatFading = false;
    }

    
    public void OpenEmoteMenu(int emoteIndex)
    {
        //currently just emotes. Will replace with menu UI
        if(emoteIndex == -1)
        {
            Debug.Log("No emote slected");
            return;
        }
        
        //Emote(emoteIndex);
        //sends a message to everyone that you are using emote
        photonView.RPC("Emote", RpcTarget.AllBuffered, emoteIndex);
    }

    [PunRPC]
    public void Emote(int emoteIndex)
    {
        Vector3 randPos;
        float x, y, z;
        for(int i = 0; i < emoteAmount; i++)
        {
            x = transform.position.x + Random.Range(-5.0f, 5.0f);
            y = transform.position.y + Random.Range(-5.0f, 5.0f);
            z = transform.position.z + Random.Range(-5.0f, 5.0f);
            randPos = new Vector3(x, y, z);
            Instantiate(GetComponent<EmoteList>().emotesList[emoteIndex], randPos, Quaternion.identity);
            //PhotonNetwork.Instantiate(EmoteList.emotePath + GetComponent<EmoteList>().emotesList[Random.Range(0, 1)].name, randPos, Quaternion.identity);
            //Instantiate(GetComponent<EmoteList>().emotesList[0], randPos, Quaternion.identity);
        }        
    }

    [PunRPC]
    public void RandomEmote()
    {
        Vector3 randPos;
        float x, y, z;
        int numEmotes = GetComponent<EmoteList>().emotesList.Count;
        for(int i = 0; i < emoteAmount; i++)
        {
            x = transform.position.x + Random.Range(-5.0f, 5.0f);
            y = transform.position.y + Random.Range(-5.0f, 5.0f);
            z = transform.position.z + Random.Range(-5.0f, 5.0f);
            randPos = new Vector3(x, y, z);
            Instantiate(GetComponent<EmoteList>().emotesList[Random.Range(0, numEmotes)], randPos, Quaternion.identity);
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
        anim = GetComponent<AttachAvatar>().avatarBodyLocation.GetComponent<AvatarInfo>().anim;
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

    public void UpdateControlLock(bool move, bool look)
    {
        canMove = move;
        canLook = look;
    }

    public void AddInteractedData(GameObject interactedObject)
    {
        Hashtable hash = new Hashtable();
        objectsInteracted.Add(interactedObject.name);
        hash.Add("InteractedList", objectsInteracted);
        //PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
    }   

    public void UpdateLoginTime()
    {
        /* Not working
        Hashtable hash = new Hashtable();        
        hash.Add("LoginTime", loginTimer);
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        //Debug.Log(PhotonNetwork.LocalPlayer.CustomProperties["LoginTime"]);
        */
    }

    public IEnumerator RefocusApplicationCursor()
    {
        yield return new WaitForFixedUpdate();
        if(!isMenuOpen && !isChatOpen)
        {
            UpdateControlLock(true, true);
            Cursor.lockState = CursorLockMode.Locked; 
            Cursor.visible = false;
        }
        
    }

    public IEnumerator UnfocusApplicationCursor()
    {
        yield return new WaitForFixedUpdate();

        Cursor.lockState = CursorLockMode.None; 
        Cursor.visible = true;
    }

    private void OnApplicationFocus(bool focusStatus) 
    {
        isAppFocused = focusStatus;
        if(!isAppFocused)
        {
            StartCoroutine(UnfocusApplicationCursor());
            Debug.Log("Unfocused");
        } else {
            StartCoroutine(RefocusApplicationCursor());
            Debug.Log("Focused");
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {
            //Debug.Log("Sending player position data + anim");
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(SessionTimer);
            if(anim)
                stream.SendNext(anim.GetFloat("speed"));     
        } else {
            //Debug.Log("Receiving player position data + anim");
            realPosition = (Vector3)stream.ReceiveNext();
            realRotation = (Quaternion)stream.ReceiveNext();
            realSessionTimer = (float)stream.ReceiveNext();
            try
            {
                anim.SetFloat("speed", (float)stream.ReceiveNext());
            } catch (Exception e) {
                //Debug.Log("Server doesn't have any info on anim class. ");
            }                
        }
                
    }
}
