using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class AttachAvatar : MonoBehaviour
{
    private GameObject playerInfo;
    public GameObject defaultPrefab;
    public GameObject playerCharacterPrefab;
    public GameObject bodyOrigin;
    public AvatarInfo avatarInfo;
    public string avatarFolder = "Avatar/";

    private AssignPlayerAvatar assigner;

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
        //Debug.Log(playerCharacterPrefab.name);
        //GameObject prefab = PrefabUtility.GetCorrespondingObjectFromOriginalSource<GameObject>(this.playerCharacterPrefab);
        //GameObject prefab = Resources.Load("Avatar/");
        //Debug.Log("Path: " + prefab.name);
        //Debug.Log("Prefab Path: " + AssetDatabase.GetAssetPath(playerCharacterPrefab)); //works
        //Debug.Log("Name of prefab: " + playerCharacterPrefab.name);
        //GameObject playerCharacter = PhotonNetwork.Instantiate(avatarFolder + playerCharacterPrefab.name , bodyOrigin.transform.position, Quaternion.identity);
        //playerCharacter.transform.parent = bodyOrigin.transform;
        foreach(AssignPlayerAvatar assign in GameObject.FindObjectsOfType(typeof(AssignPlayerAvatar)))
        {
            if(PhotonNetwork.NickName == assign.GetPlayerID())
            {
                Debug.Log("Setting anims");
                assigner = assign;   
                assigner.ChangeAvatar(avatarInfo, assigner.playerAvatarInfo);
                Destroy(assign.gameObject);
            }                            
        }

        
    }

    

    
}
