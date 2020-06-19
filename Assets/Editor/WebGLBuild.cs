#if UNITY_WEBGL
#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;
using UnityEditor;

public class WebGLBuild : MonoBehaviour
{
    /*
    Preface:    Let me begin by saying that this script exclusively just moves Photon Voice out
                of the project in order for WebGL to build. That's all it does because WebGL 
                cannot compile with Unity's Microphone library.
    */
    static string photonDir = "Assets/StoreAssets/Photon";
    static string photonVoiceDir = "PhotonVoice";
    static string unphotonVoiceDir = "Exclude~";
    static string buildName = "WebGLBuild";
    static BuildTarget buildTarget = BuildTarget.WebGL;
    static BuildOptions buildOpt = BuildOptions.Development;
    static string path;

    //Change the number of variables to anything, as long as it
    //increases the length to the scene count, which is currently 3 of them
    static string[] levels = {"1"};

    [MenuItem("WebGLBuilder/Make Release Build")]
    static void ReleaseBuild()
    {
        buildOpt = BuildOptions.None;
        CreateBuild();
    }

    [MenuItem("WebGLBuilder/Make Dev Build")]
    static void DevBuild()
    {
        buildOpt = BuildOptions.Development;
        CreateBuild();
    }

    static void CreateBuild()
    {
        UsefulShortcuts.ClearConsole();
        string excludeFolderPath = "";
        path = EditorUtility.SaveFolderPanel("Choose Location of Built Game", "", "");
        if(path =="")
        {
            Debug.LogError("Didn't select a proper directory");
            return;
        }
/*
        //before build
        if(!Directory.Exists(photonDir + "/" + unphotonVoiceDir) || true)
        {
            Debug.Log("Target Directory not created");
            excludeFolderPath = photonDir + "/" + unphotonVoiceDir;
            Debug.Log("Created a folder: " + excludeFolderPath);
            try
            {
                Debug.Log("Moving: " + AssetDatabase.MoveAsset(photonDir + "/" + photonVoiceDir, excludeFolderPath));
            } 
            catch(Exception e)
            {
                Debug.LogError("You may need to exit out of any file explorer that's opened, otherwise the following error might be more specific");
                Debug.LogError("Error: " + e);
            }
            
        } else {
            Debug.Log("Unable to create folder directory");
        }*/

        AssetDatabase.Refresh();
        

        //build
        try
        {
            Debug.Log("Starting to build.");
            StartBuild();
            Debug.Log("Build successful.");
        } 
        catch (Exception e)
        {
            //undo asset changes            
            Debug.Log("Failed because something unexpected happened. Error: " + e);            
        }
        /*
        //move files back
        Debug.Log("Moving files back");        
        AssetDatabase.Refresh();
        Directory.Move(excludeFolderPath, photonDir + "/" + photonVoiceDir);

        AssetDatabase.Refresh();

        Debug.Log("Task Finished");    */    
    }

    static void StartBuild()
    {
        if(CheckScenes.GetScenes().Count == 0)
        {
            throw new Exception("There are no scenes added");
        }
        if(levels.Length != CheckScenes.GetScenes().Count)
        {
            Debug.Log("Resizing levels array to accomodate scene list");
            Array.Resize<string>(ref levels, CheckScenes.GetScenes().Count);
        }
            
        // Get filename.
        
        if(path == "")
        {
            throw new Exception("Didn't select a correct directory");
        }

        // Gather all the scenes
        for(int i = 0; i < CheckScenes.GetScenes().Count; i++)
        {
            levels[i] = CheckScenes.GetScenes()[i];
        }
        
        // Build player.
        BuildPipeline.BuildPlayer(levels, path + "/" + buildName, buildTarget, buildOpt);        

        // Run the game (Process class from System.Diagnostics).
        System.Diagnostics.Process proc = new System.Diagnostics.Process();
        proc.StartInfo.FileName = path + "/" + buildName;
        Debug.Log("Build Directory: " + path + "/" + buildName);
        proc.Start();
    }

}
#endif
#endif