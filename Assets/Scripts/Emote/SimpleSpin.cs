using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleSpin : MonoBehaviour
{
    public float speed = 60.0f;
    public float randomSpeedMod = 0.5f;
    public float randRotation = 0.0f;
    public bool reverse = false;

    void Start()
    {
        randRotation = Random.Range(0, 360);
        if(Random.Range(0, 10) > 5)
        {
            reverse = true;
            speed *= -1.0f;
        }
            
        randomSpeedMod = Random.Range(0.8f, 2.0f);
        speed *= randomSpeedMod;
    }

    void FixedUpdate()
    {

        //transform.rotation = Quaternion.Slerp(transform.rotation, rotationTarget, Time.deltaTime * speed);
        transform.Rotate(Vector3.up * Time.deltaTime * speed);
    }
}
