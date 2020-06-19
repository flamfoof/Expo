using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEditor;

public class AttachAvatar : MonoBehaviour
{
    private GameObject playerInfo;
    public GameObject defaultPrefab;
    public GameObject playerCharacterPrefab;
    public GameObject bodyOrigin;
    public string avatarFolder = "Avatar/";

    private void Awake() {
        if(!playerInfo)
        {
            playerInfo = GameObject.FindObjectOfType<AssignPlayerAvatar>().gameObject;
            this.playerCharacterPrefab = playerInfo.GetComponent<AssignPlayerAvatar>().defaultPrefab;
        } else {
            Debug.LogError("Unable to find player Info for avatar. Will use default prefab");
            this.playerCharacterPrefab = defaultPrefab;
        }

    }

    private void Start() {
        playerCharacterPrefab = playerInfo.GetComponent<AssignPlayerAvatar>().selectedPrefab;
        GameObject prefab = PrefabUtility.GetCorrespondingObjectFromOriginalSource<GameObject>(this.playerCharacterPrefab);
        //Debug.Log("Path: " + prefab.name);
        //Debug.Log("Prefab Path: " + AssetDatabase.GetAssetPath(playerCharacterPrefab)); //works
        //Debug.Log("Name of prefab: " + playerCharacterPrefab.name);
        GameObject playerCharacter = PhotonNetwork.Instantiate(avatarFolder + prefab.name , bodyOrigin.transform.position, Quaternion.identity);
        playerCharacter.transform.parent = bodyOrigin.transform;
    }
}
