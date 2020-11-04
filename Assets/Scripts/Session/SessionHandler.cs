using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SessionHandler : MonoBehaviour
{
    public static SessionHandler instance { get; private set; }
    public string passAdress = string.Empty;

    bool isPresenter = false;
    bool isStaff = false;
    

    void Awake()
    {
        if (instance != null)
        {
            DestroyImmediate(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public bool CheckIfPresenter()
    {
        return isPresenter;
    }

    public void SetPresenter(bool isPresenter)
    {
        this.isPresenter = isPresenter;
    }

    public bool CheckIfStaff()
    {
        return isStaff;
    }

    public void SetStaff(bool isStaff)
    {
        this.isStaff = isStaff;
    }
}
