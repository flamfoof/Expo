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
        //GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //cube.transform.position = gameObject.transform.position;
        rb = gameObject.GetComponent<Rigidbody>();

        StartEmojiEffects();
    }

    private void Update()
    {
        //rb.AddForce(Vector3.up);


    }
    void StartEmojiEffects()
    {
        float forceIntensity = Random.Range(10f, 35f);
        Vector3 forceDirection = new Vector3(IgniteGameManager.localPlayer.transform.forward.x * Random.Range(-15, 15), IgniteGameManager.localPlayer.transform.forward.y * Random.Range(50, 120), IgniteGameManager.localPlayer.transform.forward.z * Random.Range(1, 20));

        rb.AddForce(forceDirection * forceIntensity);
    }
    private void OnDestroy() {
        Destroy(photonView);
    }
}
