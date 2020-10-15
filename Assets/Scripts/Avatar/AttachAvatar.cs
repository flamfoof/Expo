using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

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
    void SetPlayerCustomization(int actorNumber)
    {
        Debug.Log("Setting actor customization for: " + actorNumber);
        Player player = PhotonNetwork.CurrentRoom.GetPlayer(actorNumber);

        if(player == null)
        {
            Debug.Log("Unable to set player customization for actor: " + actorNumber);
            return;
        }

        Debug.Log("Gameobject photon actor: " + player.NickName + " is getting their avatar set");
        List<PhotonView> pv = IgniteGameManager.IgniteInstance.playerList;
        GenderList.genders gender = (GenderList.genders)((int)player.CustomProperties["AvatarGender"]);
        avatarInfo.indexSuit = (int)player.CustomProperties["AvatarBodyIndex"];
        avatarInfo.indexHead = (int)player.CustomProperties["AvatarHeadIndex"];
        Debug.Log("Gameobject photon actor: " + player.NickName + " is set to head: " + avatarInfo.indexHead + "  body: " + avatarInfo.indexSuit);
        scr_Selector selectorMale = null;
        scr_Selector_Female selectorFemale = null;

        
        if(photonView.Owner.ActorNumber == actorNumber)
        {
            Debug.Log("Actor number matches, now setting the avatars");
            GameObject selectedAvatar;            

            Debug.Log("Setting genders: " + gender.ToString());
            if(gender == GenderList.genders.Male1)
            {
                avatarInfo.maleAvatar.SetActive(true);
                avatarInfo.femaleAvatar.SetActive(false);
                avatarInfo.anim = avatarInfo.maleAvatar.GetComponent<Animator>();
                selectorMale = avatarInfo.maleAvatar.GetComponent<scr_Selector>();
                selectedAvatar = avatarInfo.maleAvatar;
                selectorMale = selectedAvatar.GetComponent<scr_Selector>();
            } else
            {
                avatarInfo.maleAvatar.SetActive(false);
                avatarInfo.femaleAvatar.SetActive(true);
                avatarInfo.anim = avatarInfo.femaleAvatar.GetComponent<Animator>();
                selectorFemale = avatarInfo.femaleAvatar.GetComponent<scr_Selector_Female>();
                selectedAvatar = avatarInfo.femaleAvatar;
                selectorFemale = selectedAvatar.GetComponent<scr_Selector_Female>();
            }

            

            Debug.Log("Setting avatar meshes");
            if(selectorMale != null)
            {   
                selectorMale.pickOneSuit(avatarInfo.indexSuit);
                selectorMale.PickOneHead(avatarInfo.indexHead);
            } else if (selectorFemale != null)
            {
                selectorFemale.pickSuit(avatarInfo.indexSuit);
                selectorFemale.pickSkin(avatarInfo.indexHead);
            }            
        } else 
        {
            Debug.Log("Setting avatar for other client");

            for(int i = 0; i < pv.Count; i++)
            {
                if(pv[i].OwnerActorNr == actorNumber)
                {
                    AttachAvatar currentPVAvatar = pv[i].GetComponent<AttachAvatar>();
                    

                    GameObject selectedAvatar;
                    
                    

                    Debug.Log("Setting genders: " + gender.ToString());
                    if(gender == GenderList.genders.Male1)
                    {
                        currentPVAvatar.avatarInfo.maleAvatar.SetActive(true);
                        currentPVAvatar.avatarInfo.femaleAvatar.SetActive(false);
                        currentPVAvatar.avatarInfo.anim = currentPVAvatar.avatarInfo.maleAvatar.GetComponent<Animator>();
                        selectedAvatar = currentPVAvatar.avatarInfo.maleAvatar;
                        selectorMale = selectedAvatar.GetComponent<scr_Selector>();
                    } else
                    {
                        currentPVAvatar.avatarInfo.maleAvatar.SetActive(false);
                        currentPVAvatar.avatarInfo.femaleAvatar.SetActive(true);
                        currentPVAvatar.avatarInfo.anim = currentPVAvatar.avatarInfo.femaleAvatar.GetComponent<Animator>();
                        selectedAvatar = currentPVAvatar.avatarInfo.femaleAvatar;
                        selectorFemale = selectedAvatar.GetComponent<scr_Selector_Female>();
                    }

                    

                    Debug.Log("Setting avatar meshes");
                    if(selectorMale != null)
                    {   
                        selectorMale.pickOneSuit(avatarInfo.indexSuit);
                        selectorMale.PickOneHead(avatarInfo.indexHead);
                    } else if (selectorFemale != null)
                    {
                        selectorFemale.pickSuit(avatarInfo.indexSuit);
                        selectorFemale.pickSkin(avatarInfo.indexHead);
                    }            

                    break;
                }
                
            }    
        }
    }
}
