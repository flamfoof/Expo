using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class CommandRing : MonoBehaviour
{
    public GameObject commandCenter;
    public IgniteGameManager gameManager;
    public UserActions player;
    public Quaternion playerHeadRotation;
    public Vector3 worldPos;
    public Vector3 mousePos;
    bool isEnable;

    public float visualDistance = 5.0f;

    void Start()
    {
        gameManager = IgniteGameManager.IgniteInstance;
        player = IgniteGameManager.localPlayer.GetComponent<UserActions>();
        gameObject.SetActive(false);
    }

    void Update()
    {
        if(isEnable)
        {
            Vector3 diff;
            Vector3 lookAtDirection;
            Vector3 mousePosNorm;
            Vector3 inputMouseNorm;
            transform.LookAt(player.playerActionRay.transform);
            diff = player.transform.position;
            transform.position = worldPos + diff;
            Debug.Log( worldPos + diff);
            mousePosNorm = Vector3.zero;
            inputMouseNorm = (Input.mousePosition - mousePos).normalized;
            //V1.x * V2.x  +  V1.y * V2.y  +  V1.z * V2.z
            /*
            Debug.Log((float)Vector3.Dot(mousePos.normalized, Input.mousePosition.normalized));
            Debug.Log("Mouse Pos: " + mousePosNorm);
            Debug.Log("New Mouse Pos: " + inputMouseNorm);*/
            lookAtDirection = player.playerActionRay.transform.forward * visualDistance;
        }
    }

    void OnEnable()
    {        
        transform.position = (player.playerActionRay.transform.forward * visualDistance) + player.playerActionRay.transform.position;
        worldPos = transform.position;
        playerHeadRotation = player.playerActionRay.transform.rotation;
        mousePos = Input.mousePosition;
        isEnable = true;
    }

    void Ondisable()
    {
        isEnable = false;
    }


}
