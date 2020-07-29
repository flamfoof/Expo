using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TutorialChatCopy : MonoBehaviour
{
    public GameObject chatView;
    public GameObject chatInstance;
    public Scrollbar[] scrollbar;
    public float canvasX = 250.0f, canvasY = 150.0f;
    public float scale = .02002778f;

    void Start()
    {
        InvokeRepeating("RespawnChat", 0.1f, 2.0f);
    }

    void RespawnChat()
    {
        if(chatInstance)
        {
            Destroy(chatInstance);
        }
        chatInstance = Instantiate(chatView);
        chatInstance.transform.SetParent(this.transform);
        chatInstance.GetComponent<RectTransform>().transform.localPosition = Vector3.zero;
        chatInstance.transform.localScale = new Vector3(scale, scale, scale);
        chatInstance.transform.localRotation = new Quaternion(0.0f, 0.0f, 0.0f, 1.0f);
        chatInstance.GetComponent<RectTransform>().sizeDelta = new Vector2(canvasX, canvasY);
        chatInstance.GetComponent<CanvasGroup>().alpha = 1.0f;
        
        Invoke("SetChatToBottom", 0.05f);
    }

    void SetChatToBottom()
    {
        scrollbar = chatInstance.GetComponentsInChildren<Scrollbar>();
        for(int i = 0; i < scrollbar.Length; i++)
        {
            chatInstance.GetComponentsInChildren<Scrollbar>()[i].value = 0.0f;
        }
    }

}
