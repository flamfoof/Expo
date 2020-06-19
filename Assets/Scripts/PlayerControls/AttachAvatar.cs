using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttachAvatar : MonoBehaviour
{
    private GameObject playerInfo;
    public GameObject defaultPrefab;
    public GameObject playerCharacterPrefab;
    public GameObject bodyOrigin;

    private void Awake() {
        if(!playerInfo)
        {
            playerInfo = GameObject.FindObjectOfType<AssignPlayerAvatar>().gameObject;
            playerCharacterPrefab = playerInfo.GetComponent<AssignPlayerAvatar>().defaultPrefab;
        } else {
            Debug.LogError("Unable to find player Info for avatar. Will use default prefab");
            playerCharacterPrefab = defaultPrefab;
        }

    }

    private void Start() {
        playerCharacterPrefab = playerInfo.GetComponent<AssignPlayerAvatar>().selectedPrefab;
        GameObject playerCharacter = Instantiate(playerCharacterPrefab, bodyOrigin.transform.position, Quaternion.identity);
        playerCharacter.transform.parent = bodyOrigin.transform;
    }
}
