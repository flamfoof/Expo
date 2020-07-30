using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractableRayIdentifier : MonoBehaviour
{
    public GameObject player;
    public Camera camera;
    public CanvasGroup canvas;
    public UIEffectsUtils effectsUtils;
    public GameObject focusedObject;
    public GameObject playerfocusedObject; //For Player Object
    public float maxDistance = 30.0f;
    public bool isUsing = false;
    Interactables.InteractableType interactType;
    public RaycastHit hit;
    //9 for interactable
    public int layerMask = 1 << 9 | 1 << 5;
    private RaycastHit[] hits;


    void Start()
    {
        camera = player.GetComponent<UserActions>().camera;
        //if on PC, attach playerActionRay to Camera, otherwise put on VR controller
        transform.SetParent(camera.transform);
        transform.localPosition = new Vector3(0, 0, 0);
    }

    void FixedUpdate()
    {
        CastRay();
    }

    private void CastRay()
    {
        //Get the player Mask
        LayerMask playerMask = LayerMask.GetMask("Player");
        // Does the ray intersect any objects excluding the player layer
        // The draw rays are more for VR if we get that set up
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, maxDistance, layerMask))
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
            //Debug.Log("Did Hit: " + hit.collider.name);
            focusedObject = hit.collider.gameObject;
            playerfocusedObject = null;
            canvas.alpha = 1;
        }
        //Detect the Player Object
        //Assign the Player to the Focused Object
        else if(Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, maxDistance, playerMask))
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.green);
            playerfocusedObject = hit.collider.gameObject;
            canvas.alpha = 1;
        }
        else {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * maxDistance, Color.white);
            focusedObject = null;
            playerfocusedObject = null;
            canvas.alpha = 0;
            //Debug.Log("Did not Hit");
        }
        /*
        //hits = Physics.RaycastAll(transform.position, transform.forward, 100.0f);
        if (hits.Length > 0)
        {
            //Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
            //Debug.Log("Did Hit: " + hit.collider.name);
            //focusedObject = hits[0].collider.gameObject;
            
            for(int i = 0; i < hits.Length; i++)
            {
                focusedObject = hits[0].collider.gameObject;
            }            
        } else {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 1000, Color.white);
            focusedObject = null;
            //Debug.Log("Did not Hit");
        }*/
    
    }

    public void UseInteractable(InputActionPhase phase)
    {
        if(focusedObject)
        {            
            //phase can be either performed, started, or cancelled.
            if(focusedObject.GetComponent<Interactables>())
            {
                switch (phase)
                {                        
                    case InputActionPhase.Performed:
                        focusedObject.GetComponent<Interactables>().Perform(phase);
                        player.GetComponent<UserActions>().AddInteractedData(focusedObject);
                        break;
                    // Checks if button has been pressed
                    case InputActionPhase.Started:
                        focusedObject.GetComponent<Interactables>().Perform(phase);
                        player.GetComponent<UserActions>().AddInteractedData(focusedObject);
                        break;
                    // Checks if button has been let go before the held time (before it's fully performed)
                    case InputActionPhase.Canceled:
                        focusedObject.GetComponent<Interactables>().Perform(phase);
                        player.GetComponent<UserActions>().AddInteractedData(focusedObject);
                        break;
                }                        
            } 
        } else {
            Debug.Log("No valid object has been selected");
        }            
    }
}
