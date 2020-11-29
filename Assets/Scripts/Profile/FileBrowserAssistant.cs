using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SimpleFileBrowser;

public class FileBrowserAssistant : MonoBehaviour
{
    public ImageUploader imageUploader;
    public string[] filters;
    private bool showFilter;
    public byte[] selectedFileBytes;
    private string filePath;
    private Texture2D profileTexture;
    

    void Start()
    {   
        if(FileBrowser.CheckPermission() != FileBrowser.Permission.Granted)
        {
            Debug.Log("Doesn't have permissions, asking for it now");
            FileBrowser.RequestPermission();
        }
        if(!imageUploader)
        {
            imageUploader = GameObject.FindObjectOfType<ImageUploader>();
        }
        
        FileBrowser.SetFilters(showFilter, filters);
        FileBrowser.AddQuickLink("Pictures", Environment.GetFolderPath( Environment.SpecialFolder.MyPictures ), null);
        profileTexture = new Texture2D(2, 2);
    }   

    public void ShowLoadDialogue()
    {
        StartCoroutine(ShowLoadDialogueCoroutine());
    }

    IEnumerator ShowLoadDialogueCoroutine()
    {
        // Show a load file dialog and wait for a response from user
		// Load file/folder: file, Allow multiple selection: true
		// Initial path: default (Documents), Title: "Load File", submit button text: "Load"
		yield return FileBrowser.WaitForLoadDialog( false, true, null, "Load File", "Load" );

		// Dialog is closed
		// Print whether the user has selected some files/folders or cancelled the operation (FileBrowser.Success)
		Debug.Log( FileBrowser.Success );

		if( FileBrowser.Success )
		{
			// Print paths of the selected files (FileBrowser.Result) (null, if FileBrowser.Success is false)
			for( int i = 0; i < FileBrowser.Result.Length; i++ )
            {
                Debug.Log( FileBrowser.Result[i] );
                filePath = FileBrowser.Result[i];
            }
				

			// Read the bytes of the first file via FileBrowserHelpers
			// Contrary to File.ReadAllBytes, this function works on Android 10+, as well
			selectedFileBytes = FileBrowserHelpers.ReadBytesFromFile( FileBrowser.Result[0] );
            imageUploader.SetImageUploadText(imageUploader.textFeedbackInfo[0]);
            imageUploader.connectButton.interactable = false;
            imageUploader.uploadButton.interactable = true;            
            profileTexture.LoadImage(selectedFileBytes);            
            profileTexture.EncodeToJPG();
            imageUploader.SetImageProfilePic(profileTexture);
		}
        
    }

}
