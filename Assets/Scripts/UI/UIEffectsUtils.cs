using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIEffectsUtils : MonoBehaviour
{
    public void FadeOutImage(Image image, float time)
    {
        StartCoroutine(FadeOut(image, time));
    }

    public void FadeOutCanvasGroup(CanvasGroup canvas, float time)
    {
        StartCoroutine(FadeOut(canvas, time));
    }

    //button push
    public void FadeOutImage(float time = 1.0f)
    {
        Debug.Log(EventSystem.current.currentSelectedGameObject.name);
        GameObject canvas = EventSystem.current.currentSelectedGameObject;
        Image image = canvas.GetComponent<Image>();
        StartCoroutine(FadeOut(image, time));
    }
    
    public IEnumerator FadeOut(CanvasGroup canvas, float time)
    {
        bool fading = true;
        float lerp = 0.0f;
        float alpha = canvas.alpha;

        while(fading)
        {
            lerp += Time.fixedDeltaTime * time;
            canvas.alpha = Mathf.Lerp(alpha, 0.0f, lerp);            
            yield return new WaitForFixedUpdate();
            
            if(canvas.alpha < 0.001f)
            {
                canvas.alpha = 0.0f;
                fading = false;
            }
        }
    }

    public IEnumerator FadeOut(Image image, float time)
    {
        bool fading = true;
        float lerp = 0.0f;
        Color tempColor = image.color;
        float alpha = tempColor.a;

        while(fading)
        {
            lerp += Time.fixedDeltaTime * time;
            tempColor.a = Mathf.Lerp(alpha, 0.0f, lerp);
            image.color = tempColor;
            yield return new WaitForFixedUpdate();
            
            if(tempColor.a < 0.001f)
            {
                tempColor.a = 0.0f;
                image.color = tempColor;
                fading = false;
            }
        }
    }

    public void FadeInImage(Image image, float time)
    {
        StartCoroutine(FadeIn(image, time));
    }

    public void FadeInCanvasGroup(CanvasGroup canvas, float time)
    {
        StartCoroutine(FadeIn(canvas, time));
    }

    //button push
    public void FadeInImage(float time = 1.0f)
    {
        Debug.Log(EventSystem.current.currentSelectedGameObject.name);
        GameObject canvas = EventSystem.current.currentSelectedGameObject;
        Image image = canvas.GetComponent<Image>();
        StartCoroutine(FadeIn(image, time));
    }
    
    public IEnumerator FadeIn(CanvasGroup canvas, float time)
    {
        bool fading = true;
        float lerp = 0.0f;
        float alpha = canvas.alpha;

        while(fading)
        {
            lerp += Time.fixedDeltaTime * time;
            canvas.alpha = Mathf.Lerp(alpha, 1.0f, lerp);            
            yield return new WaitForFixedUpdate();
            
            if(canvas.alpha > 1.0f)
            {
                canvas.alpha = 1.0f;
                fading = false;
            }
        }
    }

    public IEnumerator FadeIn(Image image, float time)
    {
        bool fading = true;
        float lerp = 0.0f;
        Color tempColor = image.color;
        float alpha = tempColor.a;

        while(fading)
        {
            lerp += Time.fixedDeltaTime * time;
            tempColor.a = Mathf.Lerp(alpha, 1.0f, lerp);
            image.color = tempColor;
            yield return new WaitForFixedUpdate();
            
            if(tempColor.a >= 1.0f)
            {
                tempColor.a = 1.0f;
                image.color = tempColor;
                fading = false;
            }
        }
    }

    public IEnumerator FadeRepeat(Image image, float time)
    {
        bool fadeMode = true;
        yield return new WaitForFixedUpdate();
        while(true)
        {
            yield return new WaitForFixedUpdate();
            if(fadeMode)
            {
                yield return StartCoroutine(FadeIn(image, time));
                fadeMode = false;
            } else 
            {
                yield return StartCoroutine(FadeOut(image, time));
                fadeMode = true;
            }
        }
    }

    public IEnumerator StopFadeRepeat(Image image, float time)
    {        
        yield return new WaitForFixedUpdate();
        StopCoroutine (FadeRepeat(image, time));
        Debug.Log("Stopped coroutine fading");
        StartCoroutine(FadeOut(image, time));    
    }
}
