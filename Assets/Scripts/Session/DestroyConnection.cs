using Byn.Unity.Examples;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class DestroyConnection : MonoBehaviour
{
    OneToMany liveStream;
    bool isClicked = false;

    void Start()
    {
        liveStream = GetComponent<OneToMany>();
        Assert.IsNotNull(liveStream);
    }

    public void StartEndStream(bool status)
    {
        if (!status)
        {
           liveStream.ShutDownServer();
        }
        else
        {
            liveStream.StartStream();
        }
    }
}
