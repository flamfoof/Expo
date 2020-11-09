using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YoutubePlayerSupporter : MonoBehaviour
{
    public GameObject clickButton;

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
        if (this.gameObject.GetComponent<ISimpleVideo>() != null)
        {
            this.gameObject.GetComponent<ISimpleVideo>().Perform(UnityEngine.InputSystem.InputActionPhase.Started);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
