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

    private void Start() 
    { 
        //Debug.Log(playerCharacterPrefab.name);
        //GameObject prefab = PrefabUtility.GetCorrespondingObjectFromOriginalSource<GameObject>(this.playerCharacterPrefab);
        //GameObject prefab = Resources.Load("Avatar/");
        //Debug.Log("Path: " + prefab.name);
        //Debug.Log("Prefab Path: " + AssetDatabase.GetAssetPath(playerCharacterPrefab)); //works
        //Debug.Log("Name of prefab: " + playerCharacterPrefab.name);
        //GameObject playerCharacter = PhotonNetwork.Instantiate(avatarFolder + playerCharacterPrefab.name , bodyOrigin.transform.position, Quaternion.identity);
        //playerCharacter.transform.parent = bodyOrigin.transform;
        /*
        foreach(AssignPlayerAvatar assign in GameObject.FindObjectsOfType(typeof(AssignPlayerAvatar)))
        {
            
            if(photonView.IsMine && PlayerPrefs.GetString(LobbyLauncherUI.playerNickname) == assign.GetPlayerID())
            {
                //Debug.Log("Setting anims");
                assigner = assign;   

                //old avatar assigner
                //assigner.ChangeAvatar(this.avatarInfo, assigner.playerAvatarInfo);


            }
            else {
                assigner = assign;
                //assigner.photonView.RPC("ChangeAvatar", RpcTarget.All, avatarInfo, assigner.playerAvatarInfo);
            }
        }*/
        assigner = AssignPlayerAvatar.instance;

        if(photonView.IsMine)
        {
            

        } else 
        {

            //assigner.photonView.RPC("ChangeAvatar", RpcTarget.All, avatarInfo, assigner.playerAvatarInfo);
        }


        
    }

    [PunRPC]
    void SetPlayerCustomization(int actorNumber, GenderList.genders gender, int indexSuit, int indexHead)
    {
        Debug.Log("Gameobject photon actor: " + photonView.gameObject.name);
        if(photonView.Owner.ActorNumber == actorNumber)
        {
            Debug.Log("Actor number matches, now setting the avatars");
            GameObject selectedAvatar;
            scr_Selector selectorMale = null;
            scr_Selector_Female selectorFemale = null;

            Debug.Log("Setting genders: " + gender.ToString());
            if(gender == GenderList.genders.Male1)
            {
                avatarInfo.maleAvatar.SetActive(true);
                avatarInfo.femaleAvatar.SetActive(false);
                avatarInfo.anim = avatarInfo.maleAvatar.GetComponent<Animator>();
                selectedAvatar = avatarInfo.maleAvatar;
                selectorMale = selectedAvatar.GetComponent<scr_Selector>();
            } else
            {
                avatarInfo.maleAvatar.SetActive(false);
                avatarInfo.femaleAvatar.SetActive(true);
                avatarInfo.anim = avatarInfo.femaleAvatar.GetComponent<Animator>();
                selectedAvatar = avatarInfo.femaleAvatar;
                selectorFemale = selectedAvatar.GetComponent<scr_Selector_Female>();
            }

            avatarInfo.indexSuit = indexSuit;
            avatarInfo.indexHead = indexHead;

            Debug.Log("Setting avatar meshes");
            if(selectorMale != null)
            {   
                Debug.Log("Maling");
                selectorMale.pickOneSuit(avatarInfo.indexSuit);
                selectorMale.PickOneHead(avatarInfo.indexHead);
            } else if (selectorFemale != null)
            {
                Debug.Log("Femaling");
                selectorFemale.pickSuit(avatarInfo.indexSuit);
                selectorFemale.pickSkin(avatarInfo.indexHead);
            }            
        }
    }
}
