using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Emote : MonoBehaviourPunCallbacks
{
    public string name;
    public float duration = 5.0f;

    Rigidbody rb;

    void Start()
    {
        Destroy(gameObject, duration);
        rb = gameObject.GetComponent<Rigidbody>();

        StartEmojiEffects();
    }

    private void Update()
    {
        //rb.AddForce(Vector3.up);


    }
    void StartEmojiEffects()
    {
        float forceIntensity = Random.Range(.5f, 5f);
        Vector3 forceDirection = new Vector3(Random.Range(-2f, 5f), 4f, Random.Range(-2f, 5f));
        rb.AddForce(forceDirection * forceIntensity);
    }
    private void OnDestroy() {
        PhotonNetwork.Destroy(photonView);
    }
}
