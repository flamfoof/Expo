using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using UnityEngine.Networking;

public class ImageUploader : MonoBehaviour
{
    static public ImageUploader share;

    private void Awake()
    {
        share = this;
    }

    public string serverURL = "";

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public IEnumerator PostImageToServer(string imgURL, string imgName)
    {
        var file = UnityWebRequest.Get(imgURL);
        yield return file.SendWebRequest();
        WWWForm form = new WWWForm();
        form.AddBinaryData("file", file.downloadHandler.data);
        form.AddField("fileName", imgName);

        var upload = UnityWebRequest.Post(serverURL, form);
        yield return upload.SendWebRequest();

        if (upload.isHttpError)
            Debug.Log(upload.error);
        else
            Debug.Log("Uploaded Successfully");

        Debug.Log(upload.downloadHandler.text);    // display whether Server got the File
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
