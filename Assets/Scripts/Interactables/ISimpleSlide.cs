using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.InputSystem;

public class ISimpleSlide : Interactables
{
    public string linkURL;
    public GameObject[] slides;
    int currentSlide = 0;
    
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
        if (currentSlide < slides.Length)
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
