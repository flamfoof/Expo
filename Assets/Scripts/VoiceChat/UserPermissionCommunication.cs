using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserPermissionCommunication : MonoBehaviour
{
    public bool allowMicrophone = false;
    public bool allowVideo = false;
    public static UserPermissionCommunication instance;

    private void Start() 
    {
        if(instance == null)
        {
            instance = this.gameObject.GetComponent<UserPermissionCommunication>();
        } else if(this.gameObject != instance)
        {
            Destroy(gameObject);
        }
    }
    public void SetMicrophone(bool status)
    {
        allowMicrophone = status;
    }

    public void SetVideo(bool status)
    {
        allowVideo = status;
    }

}
