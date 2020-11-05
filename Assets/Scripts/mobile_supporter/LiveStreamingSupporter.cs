using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiveStreamingSupporter : MonoBehaviour
{
    public GameObject clickButton;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void OnEnable()
    {
#if UNITY_IOS || UNITY_ANDROID
        clickButton.SetActive(true);
#else
        clickButton.SetActive(false);
#endif
    }

    public void ButtonClicked()
    {
        if(this.transform.GetChild(0).gameObject.GetComponent<ISimpleLiveStream>() != null)
        {
            this.transform.GetChild(0).gameObject.GetComponent<ISimpleLiveStream>().Perform(UnityEngine.InputSystem.InputActionPhase.Started);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
