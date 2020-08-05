using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BasicSceneFunctions : MonoBehaviour
{
    

    public void ChangeScene(string scene)
    {
        IgniteGameManager.IgniteInstance.LeaveRoom();
        SceneManager.LoadScene(scene);
    }
}
