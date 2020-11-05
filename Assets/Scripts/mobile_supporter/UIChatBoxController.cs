using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIChatBoxController : MonoBehaviour
{
    public GameObject sendButton;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnEnable()
    {
        Debug.Log("called");
#if UNITY_ANDROID || UNITY_EDITOR
        sendButton.SetActive(true);
#else
        sendButton.SetActive(false);
#endif
    }

    // Update is called once per frame
    void Update()
    {

    }
}
