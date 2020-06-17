     // C# editor script
     using UnityEngine;
     using UnityEditor;
     
     static class UsefulShortcuts
     {
         [MenuItem ("Tools/Clear Console %#c")] // CMD + SHIFT + C
         static public void ClearConsole () {
             // This simply does "LogEntries.Clear()" the long way:
             var logEntries = System.Type.GetType("UnityEditor.LogEntries,UnityEditor.dll");
             var clearMethod = logEntries.GetMethod("Clear", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
             clearMethod.Invoke(null,null);
         }
     
     }