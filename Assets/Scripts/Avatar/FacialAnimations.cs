using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FacialAnimations : MonoBehaviour
{
    public Texture eyeTexture;
    public Texture eyeBlinkTexture;

    public GameObject[] eyes;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("Blinking");
    }

    IEnumerator Blinking()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(2f, 8f));
            foreach (GameObject g in eyes) {
                g.GetComponent<MeshRenderer>().material.SetTexture("_MainTex", eyeBlinkTexture);
                    }
            yield return new WaitForSeconds(.2f);

            foreach (GameObject g in eyes)
            {
                g.GetComponent<MeshRenderer>().material.SetTexture("_MainTex", eyeTexture);
            }
        }
    }
}
