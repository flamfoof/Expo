#if UNITY_WEBGL
#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;
using UnityEditor;
using System.Diagnostics;

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

    //Change the number of variables to anything, as long as it
    //increases the length to the scene count, which is currently 3 of them
    static string[] levels = {"1", "2", "3"};

    [MenuItem("WebGLBuilder/Make Build")]
    static void CreateBuild()
    {
        string excludeFolder = "";
        string excludeFolderPath = "";
        UsefulShortcuts.ClearConsole();

        //before build
        if(!Directory.Exists(photonDir + "/" + unphotonVoiceDir) || true)
        {
            UnityEngine.Debug.Log("Target Directory not created");
            //excludeFolder = AssetDatabase.CreateFolder(photonDir, unphotonVoiceDir);
            //excludeFolderPath = AssetDatabase.GUIDToAssetPath(excludeFolder);
            excludeFolderPath = photonDir + "/" + unphotonVoiceDir;
            UnityEngine.Debug.Log("Created a folder: " + excludeFolderPath);
            UnityEngine.Debug.Log("Moving: " + AssetDatabase.MoveAsset(photonDir + "/" + photonVoiceDir, excludeFolderPath));
        } else{
            UnityEngine.Debug.Log("Unable to create folder directory");
        }
        AssetDatabase.Refresh();
        

        //build
        try{
            UnityEngine.Debug.Log("Starting to build.");
            StartBuild();
        } 
        catch (Exception e)
        {
            //undo asset changes            
            UnityEngine.Debug.Log("Failed because something unexpected happened. Moving all files back. Error: " + e);            
        }
        
        
        //after build 
        UnityEngine.Debug.Log("Done building");

        //move files
        UnityEngine.Debug.Log("Moving files back");        
        AssetDatabase.Refresh();
        //UnityEngine.Debug.Log("Moving back: " + AssetDatabase.MoveAsset(excludeFolderPath, photonDir + "/" + photonVoiceDir));
        //Directory.Delete(photonDir + "/" + photonVoiceDir);  
        Directory.Move(excludeFolderPath, photonDir + "/" + photonVoiceDir);

        AssetDatabase.Refresh();
        
        
    }

    static void StartBuild()
    {
        if(levels.Length != CheckScenes.GetScenes().Count)
        {
            UnityEngine.Debug.LogError("Change the length of the levels (line 17) variable according to the scene count, currently: " + CheckScenes.GetScenes().Count);
            return;
        }
            

        // Get filename.
        string path = EditorUtility.SaveFolderPanel("Choose Location of Built Game", "", "");

        for(int i = 0; i < CheckScenes.GetScenes().Count; i++)
        {
            levels[i] = CheckScenes.GetScenes()[i];
        }

        // Build player.
        BuildPipeline.BuildPlayer(levels, path + "/BuiltGame.exe", BuildTarget.WebGL, BuildOptions.None);

        // Copy a file from the project folder to the build folder, alongside the built game.
        //FileUtil.CopyFileOrDirectory("Assets/Templates/Readme.txt", path + "Readme.txt");

        // Run the game (Process class from System.Diagnostics).
        Process proc = new Process();
        proc.StartInfo.FileName = path + "/BuiltGameWebGL";
        proc.Start();
    }

}
#endif
#endif