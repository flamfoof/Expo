using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using UnityEngine.Networking;
using UnityEngine.Events;

public class ImageUploader : MonoBehaviour
{
    static public ImageUploader share;
    public Button connectButton;
    public Button selectFileButton;
    public Button uploadButton;
    public Text finishItText;
    FileStream fileStream;
    public List<string> textFeedbackInfo;
    public RawImage profileImage;
    public FileBrowserAssistant fileAssistant;
    public LobbyLauncher lobby;
    private string serverURL = "https://www.weignite.it/profile/images/index.php";

    private void Awake()
    {
        share = this;
    }

    

    // Start is called before the first frame update
    void Start()
    {
        lobby = GameObject.FindObjectOfType<LobbyLauncher>();
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
    }

    public void UploadImage()
    {
        StartCoroutine(UploadImageCoroutine());
    }

    IEnumerator UploadImageCoroutine()
    {
        Texture2D tex;
        WWWForm form = new WWWForm();
        uploadButton.interactable = false;
        tex = (Texture2D)profileImage.texture;
        byte[] bytes = null;
        bytes = tex.EncodeToJPG();
        form.AddBinaryData("myimage", bytes, lobby.nickname + ".jpg", "image/jpg");
        UnityWebRequest w = UnityWebRequest.Post(serverURL, form);
        w.timeout = 20;
        StartCoroutine(ProgressUpload(w));

        yield return w.SendWebRequest();
        
        if(w.error != null)
        {
            Debug.Log("Error: " + w.error);
            finishItText.text = w.error;
            connectButton.interactable = true;
            uploadButton.interactable = true;
        } else 
        {
            Debug.Log(w.downloadHandler.text);
            connectButton.interactable = true;
            uploadButton.interactable = true;
            finishItText.text = textFeedbackInfo[2];
        }
        w.Dispose();
    }

    IEnumerator ProgressUpload(UnityWebRequest w)
    {
        while(!w.isDone)
        {
            Debug.Log((float)w.uploadProgress);
            finishItText.text = textFeedbackInfo[1] + (w.uploadProgress * 100.0).ToString("F2");
            yield return new WaitForSeconds(0.1f);
        }
    }

    Texture2D GetTextureCopy(Texture2D source)
    {
        RenderTexture rt = RenderTexture.GetTemporary(source.width, source.height, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Linear);
        Graphics.Blit(source, rt);

        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = rt;

        Texture2D readableTexture = new Texture2D(source.width, source.height);
        readableTexture.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        readableTexture.Apply();

        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(rt);
        
        return readableTexture;
    }

    Texture2D RawToTexture2D(RenderTexture rTex)
    {
        Texture2D dest = new Texture2D(rTex.width, rTex.height, TextureFormat.RGBA32, false);
        dest.Apply(false);
        Graphics.CopyTexture(rTex, dest);
        return dest;
    }

    public void SelectFile()
    {
        fileStream = new FileStream("Path here", FileMode.Open);
        connectButton.interactable = false;
        finishItText.gameObject.SetActive(true);
        uploadButton.interactable = true;
    }

    public void Upload()
    {
        connectButton.interactable = true;
        finishItText.gameObject.SetActive(false);
        uploadButton.interactable = false;
    }

    public void SetConnectButtonActive(bool state)
    {
        connectButton.interactable = state;
    }

    public void SetImageUploadText(string text)
    {
        finishItText.text = text;
    }

    public void SetImageProfilePic(Texture2D texture)
    {
        profileImage.texture = texture;
    }

    public void UploadButton()
    {
        
    }
}
