using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DissonanceSetVolumeRange : MonoBehaviour
{
    public SphereCollider sphereTrigger;
    public GameObject playbackPrefab;

    private void Start() 
    {
        AudioSource audioSource = playbackPrefab.GetComponent<AudioSource>();

        sphereTrigger.radius = audioSource.maxDistance;
    }
}
