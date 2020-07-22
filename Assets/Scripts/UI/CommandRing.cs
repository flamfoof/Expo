using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class CommandRing : MonoBehaviour
{
    public GameObject commandCenter;
    public UserActions player;
    public Quaternion playerHeadRotation;
    public Vector3 mousePos;

    public float visualDistance = 5.0f;


    void FixedUpdate()
    {
        Vector3 lookAtDirection;
        Vector3 mousePosNorm;
        Vector3 inputMouseNorm;
        mousePosNorm = Vector3.zero;
        inputMouseNorm = (Input.mousePosition - mousePos).normalized;
        //V1.x * V2.x  +  V1.y * V2.y  +  V1.z * V2.z
        /*
        Debug.Log((float)Vector3.Dot(mousePos.normalized, Input.mousePosition.normalized));
        Debug.Log("Mouse Pos: " + mousePosNorm);
        Debug.Log("New Mouse Pos: " + inputMouseNorm);*/
        lookAtDirection = player.playerActionRay.transform.forward * visualDistance;
        
    }

    void OnEnable()
    {
        transform.LookAt(player.playerActionRay.transform);
        transform.position = (player.playerActionRay.transform.forward * visualDistance) + player.playerActionRay.transform.position;
        playerHeadRotation = player.playerActionRay.transform.rotation;
        mousePos = Input.mousePosition;
    }


}
