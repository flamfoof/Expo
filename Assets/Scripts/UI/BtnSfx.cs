using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class BtnSfx : MonoBehaviour
{
    private AudioSource audioSource;

    public AudioClip hoverSfx;
    public AudioClip clickSfx;

    void Start()
    {
        audioSource = gameObject.AddComponent(typeof(AudioSource)) as AudioSource;

        Assert.IsNotNull(audioSource);
    }

    public void HoverSound()
    {
        audioSource.PlayOneShot(hoverSfx);
    }

    public void ClickSound()
    {
        audioSource.PlayOneShot(clickSfx);
    }
}
