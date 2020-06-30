using System;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class AssignPlayerAvatar : MonoBehaviourPunCallbacks
{
    //the playerID in this case would just be the name for now
    private string playerID;
    private GenderList.genders gender;
    public GameObject male1Prefab;
    public GameObject male2Prefab;
    public GameObject female1Prefab;
    public GameObject female2Prefab;
    public GameObject defaultPrefab;
    public GameObject selectedPrefab;

    [Header("These variables are for customizing characters (later feature)")]
    public AvatarInfo playerAvatarInfo;
    /*
    public List<Hats> hats;
    public List<Shirts> shirt;
    public List<Pants> pants;
    public List<Shoes> shoes;
    public List<Accessories> accessories;
    */
    public enum ClothesType
    {
        Hat,
        Shirt,
        Pants,
        Shoe,
        Accessory
    }

    void Awake()
    {
        DontDestroyOnLoad(this);
        this.selectedPrefab = defaultPrefab;        
    }

    private void Start() {
        playerAvatarInfo = defaultPrefab.GetComponent<AvatarInfo>();

        //catch all if an avatar never loads
        //InvokeRepeating("RefreshAvatarList", 5.0f, 10.0f);
    }

    public string GetPlayerID()
    {
        return playerID;
    }
    public void SetPlayerID(string id)
    {
        this.playerID = id;
        //Debug.Log("Player id is now: " + playerID);
    }
    
    public GenderList.genders Gender {
        get{ 
            return this.gender; 
        }
        set{ 
            gender = value; 

            Hashtable hash = new Hashtable();            
            hash.Add("AvatarType", gender);
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
            Debug.Log(PhotonNetwork.LocalPlayer.CustomProperties["AvatarType"]);
            
            switch(gender)
            {
                case GenderList.genders.Male1:
                    this.selectedPrefab = male1Prefab;
                    
                    SetAvatarFeatures(selectedPrefab.GetComponent<AvatarInfo>());
                    break;
                case GenderList.genders.Female1:
                    this.selectedPrefab = female1Prefab;
                    SetAvatarFeatures(selectedPrefab.GetComponent<AvatarInfo>());
                    break;
                case GenderList.genders.Male2:
                    this.selectedPrefab = male2Prefab;
                    SetAvatarFeatures(selectedPrefab.GetComponent<AvatarInfo>());
                    break;
                case GenderList.genders.Female2:
                    this.selectedPrefab = female2Prefab;
                    SetAvatarFeatures(selectedPrefab.GetComponent<AvatarInfo>());
                    break;
                case GenderList.genders.NonBinary:
                    break;
                default:
                    break;
            }
        }
    }

    //temporary solution to pre-set avatars
    [PunRPC]
    public void SetAvatarFeatures(AvatarInfo info)
    {
        //set the basic class information from the unity components
        playerAvatarInfo = info;
        playerAvatarInfo.anim = info.anim;
        playerAvatarInfo.meshHair = info.meshHair;
        playerAvatarInfo.meshHead = info.meshHead;
        playerAvatarInfo.meshChest = info.meshChest;
        playerAvatarInfo.meshArm = info.meshArm;
        playerAvatarInfo.meshForearm = info.meshForearm;
        playerAvatarInfo.meshSpine = info.meshSpine;
        playerAvatarInfo.meshPelvis = info.meshPelvis;
        playerAvatarInfo.meshLegs = info.meshLegs;
        playerAvatarInfo.meshFeet = info.meshFeet;

        playerAvatarInfo.meshHat = info.meshHat;
        playerAvatarInfo.meshShirt = info.meshShirt;
        playerAvatarInfo.meshPants = info.meshPants;
        playerAvatarInfo.meshShoes = info.meshShoes;
        playerAvatarInfo.meshAccessories = info.meshAccessories;

        //visibility
        playerAvatarInfo.meshHair.gameObject.SetActive(info.meshHair.gameObject.activeSelf);
        playerAvatarInfo.meshHead.gameObject.SetActive(info.meshHead.gameObject.activeSelf);
        playerAvatarInfo.meshChest.gameObject.SetActive(info.meshChest.gameObject.activeSelf);
        playerAvatarInfo.meshArm.gameObject.SetActive(info.meshArm.gameObject.activeSelf);
        playerAvatarInfo.meshForearm.gameObject.SetActive(info.meshForearm.gameObject.activeSelf);
        playerAvatarInfo.meshSpine.gameObject.SetActive(info.meshSpine.gameObject.activeSelf);
        playerAvatarInfo.meshPelvis.gameObject.SetActive(info.meshPelvis.gameObject.activeSelf);
        playerAvatarInfo.meshLegs.gameObject.SetActive(info.meshLegs.gameObject.activeSelf);
        playerAvatarInfo.meshFeet.gameObject.SetActive(info.meshFeet.gameObject.activeSelf);
        playerAvatarInfo.meshHat.gameObject.SetActive(info.meshHat.gameObject.activeSelf);
        playerAvatarInfo.meshShirt.gameObject.SetActive(info.meshShirt.gameObject.activeSelf);
        playerAvatarInfo.meshPants.gameObject.SetActive(info.meshPants.gameObject.activeSelf);
        playerAvatarInfo.meshShoes.gameObject.SetActive(info.meshShoes.gameObject.activeSelf);
        playerAvatarInfo.meshAccessories.gameObject.SetActive(info.meshAccessories.gameObject.activeSelf);

        // get the materialplayerAvatarInfo.
        playerAvatarInfo.meshHair.gameObject.GetComponent<Renderer>().sharedMaterial = info.meshHair.gameObject.GetComponent<Renderer>().sharedMaterial;
        playerAvatarInfo.meshHead.gameObject.GetComponent<Renderer>().sharedMaterial = info.meshHead.gameObject.GetComponent<Renderer>().sharedMaterial;
        playerAvatarInfo.meshChest.gameObject.GetComponent<Renderer>().sharedMaterial = info.meshChest.gameObject.GetComponent<Renderer>().sharedMaterial;
        playerAvatarInfo.meshArm.gameObject.GetComponent<Renderer>().sharedMaterial = info.meshArm.gameObject.GetComponent<Renderer>().sharedMaterial;
        playerAvatarInfo.meshForearm.gameObject.GetComponent<Renderer>().sharedMaterial = info.meshForearm.gameObject.GetComponent<Renderer>().sharedMaterial;
        playerAvatarInfo.meshSpine.gameObject.GetComponent<Renderer>().sharedMaterial = info.meshSpine.gameObject.GetComponent<Renderer>().sharedMaterial;
        playerAvatarInfo.meshPelvis.gameObject.GetComponent<Renderer>().sharedMaterial = info.meshPelvis.gameObject.GetComponent<Renderer>().sharedMaterial;
        playerAvatarInfo.meshLegs.gameObject.GetComponent<Renderer>().sharedMaterial = info.meshLegs.gameObject.GetComponent<Renderer>().sharedMaterial;
        playerAvatarInfo.meshFeet.gameObject.GetComponent<Renderer>().sharedMaterial = info.meshFeet.gameObject.GetComponent<Renderer>().sharedMaterial;
        playerAvatarInfo.meshHat.gameObject.GetComponent<Renderer>().sharedMaterial = info.meshHat.gameObject.GetComponent<Renderer>().sharedMaterial;
        playerAvatarInfo.meshShirt.gameObject.GetComponent<Renderer>().sharedMaterial = info.meshShirt.gameObject.GetComponent<Renderer>().sharedMaterial;
        playerAvatarInfo.meshPants.gameObject.GetComponent<Renderer>().sharedMaterial = info.meshPants.gameObject.GetComponent<Renderer>().sharedMaterial;
        playerAvatarInfo.meshShoes.gameObject.GetComponent<Renderer>().sharedMaterial = info.meshShoes.gameObject.GetComponent<Renderer>().sharedMaterial;
        playerAvatarInfo.meshAccessories.gameObject.GetComponent<Renderer>().sharedMaterial = info.meshAccessories.gameObject.GetComponent<Renderer>().sharedMaterial;
    }

    /// <summary>
    /// Changes the Avatar's complete model to another Avatar with an AvatarInfo component
    /// </summary>
    [PunRPC]
    public void ChangeAvatar(AvatarInfo from, AvatarInfo toThis)
    {
        Debug.Log("Started assigning");
        from.anim.avatar = toThis.anim.avatar;
        from.anim.runtimeAnimatorController = toThis.anim.runtimeAnimatorController;
        from.meshHair.sharedMesh = toThis.meshHair.sharedMesh;
        from.meshHead.sharedMesh = toThis.meshHead.sharedMesh;
        from.meshChest.sharedMesh = toThis.meshChest.sharedMesh;
        from.meshArm.sharedMesh = toThis.meshArm.sharedMesh;
        from.meshForearm.sharedMesh = toThis.meshForearm.sharedMesh;
        from.meshSpine.sharedMesh = toThis.meshSpine.sharedMesh;
        from.meshPelvis.sharedMesh = toThis.meshPelvis.sharedMesh;
        from.meshLegs.sharedMesh = toThis.meshLegs.sharedMesh;
        from.meshFeet.sharedMesh = toThis.meshFeet.sharedMesh;

        from.meshHat.sharedMesh = toThis.meshHat.sharedMesh;
        from.meshShirt.sharedMesh = toThis.meshShirt.sharedMesh;
        from.meshPants.sharedMesh = toThis.meshPants.sharedMesh;
        from.meshShoes.sharedMesh = toThis.meshShoes.sharedMesh;
        from.meshAccessories.sharedMesh = toThis.meshAccessories.sharedMesh;

        //visibility
        from.meshHair.gameObject.SetActive(toThis.meshHair.gameObject.activeSelf);
        from.meshHead.gameObject.SetActive(toThis.meshHead.gameObject.activeSelf);
        from.meshChest.gameObject.SetActive(toThis.meshChest.gameObject.activeSelf);
        from.meshArm.gameObject.SetActive(toThis.meshArm.gameObject.activeSelf);
        from.meshForearm.gameObject.SetActive(toThis.meshForearm.gameObject.activeSelf);
        from.meshSpine.gameObject.SetActive(toThis.meshSpine.gameObject.activeSelf);
        from.meshPelvis.gameObject.SetActive(toThis.meshPelvis.gameObject.activeSelf);
        from.meshLegs.gameObject.SetActive(toThis.meshLegs.gameObject.activeSelf);
        from.meshFeet.gameObject.SetActive(toThis.meshFeet.gameObject.activeSelf);
        from.meshHat.gameObject.SetActive(toThis.meshHat.gameObject.activeSelf);
        from.meshShirt.gameObject.SetActive(toThis.meshShirt.gameObject.activeSelf);
        from.meshPants.gameObject.SetActive(toThis.meshPants.gameObject.activeSelf);
        from.meshShoes.gameObject.SetActive(toThis.meshShoes.gameObject.activeSelf);
        from.meshAccessories.gameObject.SetActive(toThis.meshAccessories.gameObject.activeSelf);

        // get the materialfrom.
        from.meshHair.gameObject.GetComponent<Renderer>().sharedMaterial = toThis.meshHair.gameObject.GetComponent<Renderer>().sharedMaterial;
        from.meshHead.gameObject.GetComponent<Renderer>().sharedMaterial = toThis.meshHead.gameObject.GetComponent<Renderer>().sharedMaterial;
        from.meshChest.gameObject.GetComponent<Renderer>().sharedMaterial = toThis.meshChest.gameObject.GetComponent<Renderer>().sharedMaterial;
        from.meshArm.gameObject.GetComponent<Renderer>().sharedMaterial = toThis.meshArm.gameObject.GetComponent<Renderer>().sharedMaterial;
        from.meshForearm.gameObject.GetComponent<Renderer>().sharedMaterial = toThis.meshForearm.gameObject.GetComponent<Renderer>().sharedMaterial;
        from.meshSpine.gameObject.GetComponent<Renderer>().sharedMaterial = toThis.meshSpine.gameObject.GetComponent<Renderer>().sharedMaterial;
        from.meshPelvis.gameObject.GetComponent<Renderer>().sharedMaterial = toThis.meshPelvis.gameObject.GetComponent<Renderer>().sharedMaterial;
        from.meshLegs.gameObject.GetComponent<Renderer>().sharedMaterial = toThis.meshLegs.gameObject.GetComponent<Renderer>().sharedMaterial;
        from.meshFeet.gameObject.GetComponent<Renderer>().sharedMaterial = toThis.meshFeet.gameObject.GetComponent<Renderer>().sharedMaterial;
        from.meshHat.gameObject.GetComponent<Renderer>().sharedMaterial = toThis.meshHat.gameObject.GetComponent<Renderer>().sharedMaterial;
        from.meshShirt.gameObject.GetComponent<Renderer>().sharedMaterial = toThis.meshShirt.gameObject.GetComponent<Renderer>().sharedMaterial;
        from.meshPants.gameObject.GetComponent<Renderer>().sharedMaterial = toThis.meshPants.gameObject.GetComponent<Renderer>().sharedMaterial;
        from.meshShoes.gameObject.GetComponent<Renderer>().sharedMaterial = toThis.meshShoes.gameObject.GetComponent<Renderer>().sharedMaterial;
        from.meshAccessories.gameObject.GetComponent<Renderer>().sharedMaterial = toThis.meshAccessories.gameObject.GetComponent<Renderer>().sharedMaterial;
        Debug.Log("Done assigning");
    }

    public void ChangeAvatar(AvatarInfo from, GenderList.genders gender)
    {
        AvatarInfo toThis;
        GameObject targetPrefab;

        switch(gender)
        {
            case GenderList.genders.Male1:
                targetPrefab = male1Prefab;
                break;
            case GenderList.genders.Female1:
                targetPrefab = female1Prefab;
                break;
            case GenderList.genders.Male2:
                targetPrefab = male2Prefab;
                break;
            case GenderList.genders.Female2:
                targetPrefab = female2Prefab;
                break;
            default:
                targetPrefab = defaultPrefab;
                break;
        }

        toThis = targetPrefab.GetComponent<AvatarInfo>();

        from.anim.avatar = toThis.anim.avatar;
        from.anim.runtimeAnimatorController = toThis.anim.runtimeAnimatorController;
        from.meshHair.sharedMesh = toThis.meshHair.sharedMesh;
        from.meshHead.sharedMesh = toThis.meshHead.sharedMesh;
        from.meshChest.sharedMesh = toThis.meshChest.sharedMesh;
        from.meshArm.sharedMesh = toThis.meshArm.sharedMesh;
        from.meshForearm.sharedMesh = toThis.meshForearm.sharedMesh;
        from.meshSpine.sharedMesh = toThis.meshSpine.sharedMesh;
        from.meshPelvis.sharedMesh = toThis.meshPelvis.sharedMesh;
        from.meshLegs.sharedMesh = toThis.meshLegs.sharedMesh;
        from.meshFeet.sharedMesh = toThis.meshFeet.sharedMesh;

        from.meshHat.sharedMesh = toThis.meshHat.sharedMesh;
        from.meshShirt.sharedMesh = toThis.meshShirt.sharedMesh;
        from.meshPants.sharedMesh = toThis.meshPants.sharedMesh;
        from.meshShoes.sharedMesh = toThis.meshShoes.sharedMesh;
        from.meshAccessories.sharedMesh = toThis.meshAccessories.sharedMesh;

        //visibility
        from.meshHair.gameObject.SetActive(toThis.meshHair.gameObject.activeSelf);
        from.meshHead.gameObject.SetActive(toThis.meshHead.gameObject.activeSelf);
        from.meshChest.gameObject.SetActive(toThis.meshChest.gameObject.activeSelf);
        from.meshArm.gameObject.SetActive(toThis.meshArm.gameObject.activeSelf);
        from.meshForearm.gameObject.SetActive(toThis.meshForearm.gameObject.activeSelf);
        from.meshSpine.gameObject.SetActive(toThis.meshSpine.gameObject.activeSelf);
        from.meshPelvis.gameObject.SetActive(toThis.meshPelvis.gameObject.activeSelf);
        from.meshLegs.gameObject.SetActive(toThis.meshLegs.gameObject.activeSelf);
        from.meshFeet.gameObject.SetActive(toThis.meshFeet.gameObject.activeSelf);
        from.meshHat.gameObject.SetActive(toThis.meshHat.gameObject.activeSelf);
        from.meshShirt.gameObject.SetActive(toThis.meshShirt.gameObject.activeSelf);
        from.meshPants.gameObject.SetActive(toThis.meshPants.gameObject.activeSelf);
        from.meshShoes.gameObject.SetActive(toThis.meshShoes.gameObject.activeSelf);
        from.meshAccessories.gameObject.SetActive(toThis.meshAccessories.gameObject.activeSelf);

        // get the materialfrom.
        from.meshHair.gameObject.GetComponent<Renderer>().sharedMaterial = toThis.meshHair.gameObject.GetComponent<Renderer>().sharedMaterial;
        from.meshHead.gameObject.GetComponent<Renderer>().sharedMaterial = toThis.meshHead.gameObject.GetComponent<Renderer>().sharedMaterial;
        from.meshChest.gameObject.GetComponent<Renderer>().sharedMaterial = toThis.meshChest.gameObject.GetComponent<Renderer>().sharedMaterial;
        from.meshArm.gameObject.GetComponent<Renderer>().sharedMaterial = toThis.meshArm.gameObject.GetComponent<Renderer>().sharedMaterial;
        from.meshForearm.gameObject.GetComponent<Renderer>().sharedMaterial = toThis.meshForearm.gameObject.GetComponent<Renderer>().sharedMaterial;
        from.meshSpine.gameObject.GetComponent<Renderer>().sharedMaterial = toThis.meshSpine.gameObject.GetComponent<Renderer>().sharedMaterial;
        from.meshPelvis.gameObject.GetComponent<Renderer>().sharedMaterial = toThis.meshPelvis.gameObject.GetComponent<Renderer>().sharedMaterial;
        from.meshLegs.gameObject.GetComponent<Renderer>().sharedMaterial = toThis.meshLegs.gameObject.GetComponent<Renderer>().sharedMaterial;
        from.meshFeet.gameObject.GetComponent<Renderer>().sharedMaterial = toThis.meshFeet.gameObject.GetComponent<Renderer>().sharedMaterial;
        from.meshHat.gameObject.GetComponent<Renderer>().sharedMaterial = toThis.meshHat.gameObject.GetComponent<Renderer>().sharedMaterial;
        from.meshShirt.gameObject.GetComponent<Renderer>().sharedMaterial = toThis.meshShirt.gameObject.GetComponent<Renderer>().sharedMaterial;
        from.meshPants.gameObject.GetComponent<Renderer>().sharedMaterial = toThis.meshPants.gameObject.GetComponent<Renderer>().sharedMaterial;
        from.meshShoes.gameObject.GetComponent<Renderer>().sharedMaterial = toThis.meshShoes.gameObject.GetComponent<Renderer>().sharedMaterial;
        from.meshAccessories.gameObject.GetComponent<Renderer>().sharedMaterial = toThis.meshAccessories.gameObject.GetComponent<Renderer>().sharedMaterial;
    }

/*
    [Serializable]    
    public class Hats
    {
        public string name;
        public List<ClothesType> clothesType;
    }

    [Serializable]    
    public class Shirts
    {
        public string name;
        public List<ClothesType> clothesType;
    }

    [Serializable]    
    public class Pants
    {
        public string name;
        public List<ClothesType> clothesType;
    }

    [Serializable]    
    public class Shoes
    {
        public string name;
        public List<ClothesType> clothesType;
    }

    [Serializable]    
    public class Accessories
    {
        public string name;
        public List<ClothesType> clothesType;
    }
 */
    public void RefreshAvatarList()
    {
        int count = 0;
        bool stillDefault = false;
        foreach(PhotonView pv in GameObject.FindObjectsOfType(typeof(PhotonView)))
        {
            if(pv.gameObject.GetComponent<UserActions>())
            {
                if( pv.GetComponent<AttachAvatar>().avatarBodyLocation.GetComponent<AvatarInfo>().meshHair.sharedMesh == 
                    defaultPrefab.GetComponent<AvatarInfo>().meshHair.sharedMesh)
                {
                    stillDefault = true;
                }
                count++;
            }                     
        }

        if(!stillDefault)
            return;     

        Invoke("RefreshAvatars", 1.5f);        
    }

    public void RefreshAvatars()
    {
        //update the meshes
        //sorry it's messy!                   
        int count = 0;
                    
        
        foreach(PhotonView pv in GameObject.FindObjectsOfType(typeof(PhotonView)))
        {
            Player pl = pv.Owner;
            if(pv.gameObject.GetComponent<UserActions>())
            {
                if( pv.GetComponent<AttachAvatar>().avatarBodyLocation.GetComponent<AvatarInfo>().meshHair.sharedMesh == 
                    defaultPrefab.GetComponent<AvatarInfo>().meshHair.sharedMesh)
                {
                    Debug.Log(pv.Owner.NickName + " has selected their character: " + 
                        (GenderList.genders)PhotonNetwork.PlayerList[count].CustomProperties["AvatarType"]);
                    ChangeAvatar(pv.gameObject.GetComponent<AttachAvatar>().avatarBodyLocation.GetComponent<AvatarInfo>(), 
                        (GenderList.genders)PhotonNetwork.PlayerList[count].CustomProperties["AvatarType"]);
                }
                count++;
            }            
        }
    }

    public void NetworkAvatarUpdate()
    {
        Hashtable hash = new Hashtable();            
        hash.Add("AvatarType", Gender);
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
    }

    public void PrintAvatarList()
    {
        int count = 0;
        foreach(PhotonView pv in GameObject.FindObjectsOfType(typeof(PhotonView)))
        {
            if(pv.gameObject.GetComponent<UserActions>())
            {
                Debug.Log(pv.Owner.ActorNumber + " has selected: " + 
                    (GenderList.genders)pv.Owner.CustomProperties["AvatarType"]);                
                count++;
            }            
        }

    }
}
