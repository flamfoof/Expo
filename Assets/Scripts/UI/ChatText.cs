using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatText : MonoBehaviour
{
 
    /// <summary>
    /// References to the "Text" prefab.
    /// 
    /// Needs to contain RectTransform and Text element.
    /// </summary>
    public GameObject textOutputPrefab;



    /// <summary>
    /// Reference to the own rect transform
    /// </summary>
    private RectTransform mOwnTransform;

    /// <summary>
    /// Number of messages until the older messages will be deleted.
    /// </summary>
    private int mMaxMessages = 50;


    private int mCounter = 0;

    private void Awake()
    {
        mOwnTransform = this.GetComponent<RectTransform>();
    }

    private void Start()
    {
        foreach(var v in mOwnTransform.GetComponentsInChildren<RectTransform>())
        {
            if(v != mOwnTransform)
            {
                v.name = "Element " + mCounter;
                mCounter++;
            }
        }
    }

    /// <summary>
    /// Allows the Chatapp to add new entires to the list
    /// </summary>
    /// <param name="text">Text to be added</param>
    public void AddTextEntry(string text)
    {
        GameObject ngp = Instantiate(textOutputPrefab);
        Text t = ngp.GetComponent<Text>();
        t.text = text;
        Debug.Log(text);
        RectTransform transform = ngp.GetComponent<RectTransform>();
        transform.SetParent(mOwnTransform, false);

        GameObject go = transform.gameObject;
        go.name = ngp.name + mCounter;
        mCounter++;
    }
    

    /// <summary>
    /// Destroys old messages if needed and repositions the existing messages.
    /// </summary>
    private void Update()
    {
        int destroy = mOwnTransform.childCount - mMaxMessages;
        for(int i = 0; i < destroy; i++)
        {
            var child = mOwnTransform.GetChild(i).gameObject;
            Destroy(child);
        }
    }

}
