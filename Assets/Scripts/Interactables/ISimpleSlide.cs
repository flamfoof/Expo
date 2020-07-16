using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using Photon.Pun;
using Photon.Realtime;

public class ISimpleSlide : Interactables
{
    public string linkURL;
    public GameObject[] slides;
    public Image slideImage;
    int currentSlide = 0;
    public int slideCount = 0;
    public Sprite[] images;

    private void Start() 
    {
        slideCount = images.Length;    
    }
    
    public override void Perform(InputActionPhase phase)
    {
        if (phase == InputActionPhase.Started)
        {
            NextSlide();
        }
    }

    public void NextSlide()
    {
        print(currentSlide);
        print(slides.Length);

        if (currentSlide < slides.Length-1)
        {

            slides[currentSlide].SetActive(false);

            currentSlide++;

            slides[currentSlide].SetActive(true);
        }
        else
        {
            slides[currentSlide].SetActive(false);

            currentSlide = 0;

            slides[currentSlide].SetActive(true);
        }
    }

}
