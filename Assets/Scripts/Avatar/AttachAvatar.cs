using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class AttachAvatar : MonoBehaviourPunCallbacks
{
    private GameObject playerInfo;
    public GameObject defaultPrefab;
    public GameObject playerCharacterPrefab;
    public GameObject avatarBodyLocation;
    public AvatarInfo avatarInfo;
    public string avatarFolder = "Avatar/";

    public AssignPlayerAvatar assigner;

    private void Start() {
        
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
            
            if(photonView.IsMine && PlayerPrefs.GetString(LobbyLauncherUI.playerNickname) == assign.GetPlayerID())
            {
                //Debug.Log("Setting anims");
                assigner = assign;   
                assigner.ChangeAvatar(this.avatarInfo, assigner.playerAvatarInfo);
            } else {
                assigner = assign;
                //assigner.photonView.RPC("ChangeAvatar", RpcTarget.All, avatarInfo, assigner.playerAvatarInfo);
            }
        }

        
    }
}
