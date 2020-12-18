using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CommandButton : MonoBehaviour
{
    public virtual void Click()
    {

    }

    public virtual void Click(bool setToggle)
    {

    }

    public virtual void Hover()
    {
        GetComponent<Button>().Select();
    }
}
