using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;


public class CheckScenes : MonoBehaviour
{

    [MenuItem("Tools/Scene Check")]
    static void CheckAllScenes()
    {
        UsefulShortcuts.ClearConsole();
        List<string> scenes = new List<string>();
        int counter = 0;
        foreach(UnityEditor.EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
        {
            Debug.Log("scene " + counter + ": " + scene.path);
            counter++;
        }
    }

    static public List<string> GetScenes()
    {
        List<string> scenes = new List<string>();

        foreach(UnityEditor.EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
        {
            scenes.Add(scene.path);
        }

        return scenes;
    }



}
