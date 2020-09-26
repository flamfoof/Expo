using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FacialAnimations : MonoBehaviour
{
    public Texture eyeTexture;
    public Texture eyeBlinkTexture;
    public Texture mouthOpenTexture;
    public Texture mouthCloseTexture;

    public GameObject[] eyes;
    public GameObject[] nose;
    public GameObject mouth;


    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("Blinking");
    }

    IEnumerator Blinking()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(2f, 5f));
            foreach (GameObject g in eyes)
            {
                g.GetComponent<MeshRenderer>().material.SetTexture("_MainTex", eyeBlinkTexture);
            }
            yield return new WaitForSeconds(.2f);

            foreach (GameObject g in eyes)
            {
                g.GetComponent<MeshRenderer>().material.SetTexture("_MainTex", eyeTexture);
            }
        }
    }

    public void UpdateCharacterFace(string faceType)
    {
        if (faceType == "light")
        {
            foreach (GameObject eye in eyes)
            {
                eye.GetComponent<MeshRenderer>().material.EnableKeyword("_EMISSION");
            }
            Color lightSkin = new Color(0.8867924f, 0.6806311f, 0.5730687f);
            nose[0].GetComponent<MeshRenderer>().material.SetColor("_Color", lightSkin);
            nose[1].GetComponent<MeshRenderer>().material.SetColor("_Color", lightSkin);
            nose[2].GetComponent<MeshRenderer>().material.SetColor("_Color", lightSkin);

        }
        else
        {
            print("darkskin");
            foreach (GameObject eye in eyes)
            {
                eye.GetComponent<MeshRenderer>().material.DisableKeyword("_EMISSION");
            }

            Color darkSkin = new Color(0.5537736f, 0.3422901f, 0.2319509f);
            //darken all the parts of the nose
            nose[0].GetComponent<MeshRenderer>().material.SetColor("_Color", darkSkin);
            nose[2].GetComponent<MeshRenderer>().material.SetColor("_Color", darkSkin);
            nose[1].GetComponent<MeshRenderer>().material.SetColor("_Color", darkSkin);

        }

    }
}