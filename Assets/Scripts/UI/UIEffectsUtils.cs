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
        
        //Debug.Log("faded in");
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
        
        //Debug.Log("faded out");
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
        
        //Debug.Log("faded in");
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
            
            if(tempColor.a > 1.0f)
            {
                tempColor.a = 1.0f;
                image.color = tempColor;
                fading = false;
            }
        }
        
        //Debug.Log("faded out");
    }

    public static IEnumerator AlphaFadeOut(CanvasGroup infoCanvasGroup, float start,float end,float duration)
    {
        //StopCoroutine("AlphaFadeOut");
        float counter = 0f;
        while(counter < duration)
        {
            counter += Time.deltaTime;
            infoCanvasGroup.alpha = Mathf.Lerp(start,end,counter/duration);
            yield return null;
        }
    }
}
