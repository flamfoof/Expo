using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Assertions;

public class PlayPauseUI : MonoBehaviour
{
    [SerializeField]
    Image coverImage;

    CanvasGroup UIObject;

    public Sprite play, pause;

    private void Awake()
    {
        UIObject = GetComponent<CanvasGroup>();

        Assert.IsNotNull(UIObject);
        Assert.IsNotNull(coverImage);
    }

    public void SetVisibility(bool show , bool playStatus = true)
    {
        coverImage.sprite = playStatus ? pause : play;

        if (UIObject == null)
        {
            Debug.LogError("UIObject is null");
        }

        UIObject.alpha = show ? 1f : 0;
    }
}
