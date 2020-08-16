using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserPermissionCommunication : MonoBehaviour
{
    public bool allowMicrophone = false;
    public bool allowVideo = false;

    public void SetMicrophone(bool status)
    {
        allowMicrophone = status;
    }

    public void SetVideo(bool status)
    {
        allowVideo = status;
    }

}
