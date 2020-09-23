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
            nose[0].GetComponent<MeshRenderer>().material.SetColor("_Color", new Color(0.8867924f, 0.6806311f, 0.5730687f));

        }
        else
        {
            foreach (GameObject eye in eyes)
            {
                eye.GetComponent<MeshRenderer>().material.DisableKeyword("_EMISSION");
            }
            nose[0].GetComponent<MeshRenderer>().material.SetColor("_Color", new Color(0.6037736f, 0.3922901f, 0.2819509f));

        }

    }
}