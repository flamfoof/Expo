using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class AssignPlayerAvatar : MonoBehaviourPunCallbacks
{
    //the playerID in this case would just be the name for now
    private string playerID;
    public static AssignPlayerAvatar instance;
    private GenderList.genders gender;
    
    public GameObject maleCharPrefab;
    public GameObject femaleCharPrefab;

    public GameObject selectedPrefab;
    public GameObject defaultPrefab;

    
    public int bodyIndex;
    public int headIndex;

    [Header("These variables are for customizing characters (later feature)")]
    public AvatarInfo playerAvatarInfo;
    public string profilePicLink = "";

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
        
        Gender = GenderList.genders.Male1;

        if(instance != null)
        {
            Destroy(this.gameObject);
        } else 
        {
            instance = this.gameObject.GetComponent<AssignPlayerAvatar>();
        }

        this.selectedPrefab = defaultPrefab;
    }

    private void Start() {
        string profilePic = PlayerPrefs.GetString("ProfilePic", "");
        playerAvatarInfo = defaultPrefab.GetComponent<AvatarInfo>();
        Debug.Log(instance.name);
        if(profilePicLink != "")
        {
            Debug.Log("Profile pic exists as: " + profilePicLink);
            if(profilePic == "")
            {
                profilePicLink = profilePic;
                Debug.Log("Profile pic is now: " + profilePic);
                //load image from local or download from online
            }
        } else 
        {
            Debug.Log("There is no profile picture set");
        }
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
            //Debug.Log(PhotonNetwork.LocalPlayer.CustomProperties["AvatarType"]);
            
            switch(gender)
            {
                case GenderList.genders.Male1:
                    this.selectedPrefab = maleCharPrefab;
                    break;
                case GenderList.genders.Female:
                    this.selectedPrefab = femaleCharPrefab;
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

        //playerAvatarInfo.meshHair = info.meshHair;
        //playerAvatarInfo.meshHead = info.meshHead;
        //playerAvatarInfo.meshChest = info.meshChest;
        //playerAvatarInfo.meshArm = info.meshArm;
        //playerAvatarInfo.meshForearm = info.meshForearm;
        //playerAvatarInfo.meshSpine = info.meshSpine;
        //playerAvatarInfo.meshPelvis = info.meshPelvis;
        //playerAvatarInfo.meshLegs = info.meshLegs;
        //playerAvatarInfo.meshFeet = info.meshFeet;

        //playerAvatarInfo.meshHat = info.meshHat;
        //playerAvatarInfo.meshShirt = info.meshShirt;
        //playerAvatarInfo.meshPants = info.meshPants;
        //playerAvatarInfo.meshShoes = info.meshShoes;
        //playerAvatarInfo.meshAccessories = info.meshAccessories;

        //Updated 
        playerAvatarInfo.meshLeftEye = info.meshLeftEye;
        playerAvatarInfo.meshRightEye = info.meshRightEye;
        playerAvatarInfo.meshBody = info.meshBody;
        playerAvatarInfo.meshHead = info.meshHead;
        playerAvatarInfo.meshLowerTeeth = info.meshLowerTeeth;
        playerAvatarInfo.meshUpperTeeth = info.meshUpperTeeth;

        //visibility
        //playerAvatarInfo.meshHair.gameObject.SetActive(info.meshHair.gameObject.activeSelf);
        //playerAvatarInfo.meshHead.gameObject.SetActive(info.meshHead.gameObject.activeSelf);
        //playerAvatarInfo.meshChest.gameObject.SetActive(info.meshChest.gameObject.activeSelf);
        //playerAvatarInfo.meshArm.gameObject.SetActive(info.meshArm.gameObject.activeSelf);
        //playerAvatarInfo.meshForearm.gameObject.SetActive(info.meshForearm.gameObject.activeSelf);
        //playerAvatarInfo.meshSpine.gameObject.SetActive(info.meshSpine.gameObject.activeSelf);
        //playerAvatarInfo.meshPelvis.gameObject.SetActive(info.meshPelvis.gameObject.activeSelf);
        //playerAvatarInfo.meshLegs.gameObject.SetActive(info.meshLegs.gameObject.activeSelf);
        //playerAvatarInfo.meshFeet.gameObject.SetActive(info.meshFeet.gameObject.activeSelf);
        //playerAvatarInfo.meshHat.gameObject.SetActive(info.meshHat.gameObject.activeSelf);
        //playerAvatarInfo.meshShirt.gameObject.SetActive(info.meshShirt.gameObject.activeSelf);
        //playerAvatarInfo.meshPants.gameObject.SetActive(info.meshPants.gameObject.activeSelf);
        //playerAvatarInfo.meshShoes.gameObject.SetActive(info.meshShoes.gameObject.activeSelf);
        //playerAvatarInfo.meshAccessories.gameObject.SetActive(info.meshAccessories.gameObject.activeSelf);

        //Updated
        playerAvatarInfo.meshLeftEye.gameObject.SetActive(info.meshLeftEye.gameObject.activeSelf);
        playerAvatarInfo.meshHead.gameObject.SetActive(info.meshHead.gameObject.activeSelf);
        playerAvatarInfo.meshRightEye.gameObject.SetActive(info.meshRightEye.gameObject.activeSelf);
        playerAvatarInfo.meshBody.gameObject.SetActive(info.meshBody.gameObject.activeSelf);
        playerAvatarInfo.meshLowerTeeth.gameObject.SetActive(info.meshLowerTeeth.gameObject.activeSelf);
        playerAvatarInfo.meshUpperTeeth.gameObject.SetActive(info.meshUpperTeeth.gameObject.activeSelf);

        // get the materialplayerAvatarInfo.
        //playerAvatarInfo.meshHair.gameObject.GetComponent<Renderer>().sharedMaterial = info.meshHair.gameObject.GetComponent<Renderer>().sharedMaterial;
        //playerAvatarInfo.meshHead.gameObject.GetComponent<Renderer>().sharedMaterial = info.meshHead.gameObject.GetComponent<Renderer>().sharedMaterial;
        //playerAvatarInfo.meshChest.gameObject.GetComponent<Renderer>().sharedMaterial = info.meshChest.gameObject.GetComponent<Renderer>().sharedMaterial;
        //playerAvatarInfo.meshArm.gameObject.GetComponent<Renderer>().sharedMaterial = info.meshArm.gameObject.GetComponent<Renderer>().sharedMaterial;
        //playerAvatarInfo.meshForearm.gameObject.GetComponent<Renderer>().sharedMaterial = info.meshForearm.gameObject.GetComponent<Renderer>().sharedMaterial;
        //playerAvatarInfo.meshSpine.gameObject.GetComponent<Renderer>().sharedMaterial = info.meshSpine.gameObject.GetComponent<Renderer>().sharedMaterial;
        //playerAvatarInfo.meshPelvis.gameObject.GetComponent<Renderer>().sharedMaterial = info.meshPelvis.gameObject.GetComponent<Renderer>().sharedMaterial;
        //playerAvatarInfo.meshLegs.gameObject.GetComponent<Renderer>().sharedMaterial = info.meshLegs.gameObject.GetComponent<Renderer>().sharedMaterial;
        //playerAvatarInfo.meshFeet.gameObject.GetComponent<Renderer>().sharedMaterial = info.meshFeet.gameObject.GetComponent<Renderer>().sharedMaterial;
        //playerAvatarInfo.meshHat.gameObject.GetComponent<Renderer>().sharedMaterial = info.meshHat.gameObject.GetComponent<Renderer>().sharedMaterial;
        //playerAvatarInfo.meshShirt.gameObject.GetComponent<Renderer>().sharedMaterial = info.meshShirt.gameObject.GetComponent<Renderer>().sharedMaterial;
        //playerAvatarInfo.meshPants.gameObject.GetComponent<Renderer>().sharedMaterial = info.meshPants.gameObject.GetComponent<Renderer>().sharedMaterial;
        //playerAvatarInfo.meshShoes.gameObject.GetComponent<Renderer>().sharedMaterial = info.meshShoes.gameObject.GetComponent<Renderer>().sharedMaterial;
        //playerAvatarInfo.meshAccessories.gameObject.GetComponent<Renderer>().sharedMaterial = info.meshAccessories.gameObject.GetComponent<Renderer>().sharedMaterial;

        //Updated
        playerAvatarInfo.meshLeftEye.gameObject.GetComponent<Renderer>().sharedMaterial = info.meshLeftEye.gameObject.GetComponent<Renderer>().sharedMaterial;
        playerAvatarInfo.meshHead.gameObject.GetComponent<Renderer>().sharedMaterial = info.meshHead.gameObject.GetComponent<Renderer>().sharedMaterial;
        playerAvatarInfo.meshRightEye.gameObject.GetComponent<Renderer>().sharedMaterial = info.meshRightEye.gameObject.GetComponent<Renderer>().sharedMaterial;
        playerAvatarInfo.meshBody.gameObject.GetComponent<Renderer>().sharedMaterial = info.meshBody.gameObject.GetComponent<Renderer>().sharedMaterial;
        playerAvatarInfo.meshLowerTeeth.gameObject.GetComponent<Renderer>().sharedMaterial = info.meshLowerTeeth.gameObject.GetComponent<Renderer>().sharedMaterial;
        playerAvatarInfo.meshUpperTeeth.gameObject.GetComponent<Renderer>().sharedMaterial = info.meshUpperTeeth.gameObject.GetComponent<Renderer>().sharedMaterial;
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
        //from.meshHair.sharedMesh = toThis.meshHair.sharedMesh;
        //from.meshHead.sharedMesh = toThis.meshHead.sharedMesh;
        //from.meshChest.sharedMesh = toThis.meshChest.sharedMesh;
        //from.meshArm.sharedMesh = toThis.meshArm.sharedMesh;
        //from.meshForearm.sharedMesh = toThis.meshForearm.sharedMesh;
        //from.meshSpine.sharedMesh = toThis.meshSpine.sharedMesh;
        //from.meshPelvis.sharedMesh = toThis.meshPelvis.sharedMesh;
        //from.meshLegs.sharedMesh = toThis.meshLegs.sharedMesh;
        //from.meshFeet.sharedMesh = toThis.meshFeet.sharedMesh;
        //from.meshHat.sharedMesh = toThis.meshHat.sharedMesh;
        //from.meshShirt.sharedMesh = toThis.meshShirt.sharedMesh;
        //from.meshPants.sharedMesh = toThis.meshPants.sharedMesh;
        //from.meshShoes.sharedMesh = toThis.meshShoes.sharedMesh;
        //from.meshAccessories.sharedMesh = toThis.meshAccessories.sharedMesh;

        //Updated
        from.meshRightEye.sharedMesh = toThis.meshRightEye.sharedMesh;
        from.meshHead.sharedMesh = toThis.meshHead.sharedMesh;
        from.meshLeftEye.sharedMesh = toThis.meshLeftEye.sharedMesh;
        from.meshBody.sharedMesh = toThis.meshBody.sharedMesh;
        from.meshUpperTeeth.sharedMesh = toThis.meshUpperTeeth.sharedMesh;
        from.meshLowerTeeth.sharedMesh = toThis.meshLowerTeeth.sharedMesh;

        //visibility
        //from.meshHair.gameObject.SetActive(toThis.meshHair.gameObject.activeSelf);
        //from.meshHead.gameObject.SetActive(toThis.meshHead.gameObject.activeSelf);
        //from.meshChest.gameObject.SetActive(toThis.meshChest.gameObject.activeSelf);
        //from.meshArm.gameObject.SetActive(toThis.meshArm.gameObject.activeSelf);
        //from.meshForearm.gameObject.SetActive(toThis.meshForearm.gameObject.activeSelf);
        //from.meshSpine.gameObject.SetActive(toThis.meshSpine.gameObject.activeSelf);
        //from.meshPelvis.gameObject.SetActive(toThis.meshPelvis.gameObject.activeSelf);
        //from.meshLegs.gameObject.SetActive(toThis.meshLegs.gameObject.activeSelf);
        //from.meshFeet.gameObject.SetActive(toThis.meshFeet.gameObject.activeSelf);
        //from.meshHat.gameObject.SetActive(toThis.meshHat.gameObject.activeSelf);
        //from.meshShirt.gameObject.SetActive(toThis.meshShirt.gameObject.activeSelf);
        //from.meshPants.gameObject.SetActive(toThis.meshPants.gameObject.activeSelf);
        //from.meshShoes.gameObject.SetActive(toThis.meshShoes.gameObject.activeSelf);
        //from.meshAccessories.gameObject.SetActive(toThis.meshAccessories.gameObject.activeSelf);

        //Upated
        from.meshLeftEye.gameObject.SetActive(toThis.meshLeftEye.gameObject.activeSelf);
        from.meshHead.gameObject.SetActive(toThis.meshHead.gameObject.activeSelf);
        from.meshRightEye.gameObject.SetActive(toThis.meshRightEye.gameObject.activeSelf);
        from.meshBody.gameObject.SetActive(toThis.meshBody.gameObject.activeSelf);
        from.meshLowerTeeth.gameObject.SetActive(toThis.meshLowerTeeth.gameObject.activeSelf);
        from.meshUpperTeeth.gameObject.SetActive(toThis.meshUpperTeeth.gameObject.activeSelf);

        // get the materialfrom.
        //from.meshHair.gameObject.GetComponent<Renderer>().sharedMaterial = toThis.meshHair.gameObject.GetComponent<Renderer>().sharedMaterial;
        //from.meshHead.gameObject.GetComponent<Renderer>().sharedMaterial = toThis.meshHead.gameObject.GetComponent<Renderer>().sharedMaterial;
        //from.meshChest.gameObject.GetComponent<Renderer>().sharedMaterial = toThis.meshChest.gameObject.GetComponent<Renderer>().sharedMaterial;
        //from.meshArm.gameObject.GetComponent<Renderer>().sharedMaterial = toThis.meshArm.gameObject.GetComponent<Renderer>().sharedMaterial;
        //from.meshForearm.gameObject.GetComponent<Renderer>().sharedMaterial = toThis.meshForearm.gameObject.GetComponent<Renderer>().sharedMaterial;
        //from.meshSpine.gameObject.GetComponent<Renderer>().sharedMaterial = toThis.meshSpine.gameObject.GetComponent<Renderer>().sharedMaterial;
        //from.meshPelvis.gameObject.GetComponent<Renderer>().sharedMaterial = toThis.meshPelvis.gameObject.GetComponent<Renderer>().sharedMaterial;
        //from.meshLegs.gameObject.GetComponent<Renderer>().sharedMaterial = toThis.meshLegs.gameObject.GetComponent<Renderer>().sharedMaterial;
        //from.meshFeet.gameObject.GetComponent<Renderer>().sharedMaterial = toThis.meshFeet.gameObject.GetComponent<Renderer>().sharedMaterial;
        //from.meshHat.gameObject.GetComponent<Renderer>().sharedMaterial = toThis.meshHat.gameObject.GetComponent<Renderer>().sharedMaterial;
        //from.meshShirt.gameObject.GetComponent<Renderer>().sharedMaterial = toThis.meshShirt.gameObject.GetComponent<Renderer>().sharedMaterial;
        //from.meshPants.gameObject.GetComponent<Renderer>().sharedMaterial = toThis.meshPants.gameObject.GetComponent<Renderer>().sharedMaterial;
        //from.meshShoes.gameObject.GetComponent<Renderer>().sharedMaterial = toThis.meshShoes.gameObject.GetComponent<Renderer>().sharedMaterial;
        //from.meshAccessories.gameObject.GetComponent<Renderer>().sharedMaterial = toThis.meshAccessories.gameObject.GetComponent<Renderer>().sharedMaterial;

        //Updated
        from.meshRightEye.gameObject.GetComponent<Renderer>().sharedMaterial = toThis.meshRightEye.gameObject.GetComponent<Renderer>().sharedMaterial;
        from.meshHead.gameObject.GetComponent<Renderer>().sharedMaterial = toThis.meshHead.gameObject.GetComponent<Renderer>().sharedMaterial;
        from.meshLeftEye.gameObject.GetComponent<Renderer>().sharedMaterial = toThis.meshLeftEye.gameObject.GetComponent<Renderer>().sharedMaterial;
        from.meshBody.gameObject.GetComponent<Renderer>().sharedMaterial = toThis.meshBody.gameObject.GetComponent<Renderer>().sharedMaterial;
        from.meshUpperTeeth.gameObject.GetComponent<Renderer>().sharedMaterial = toThis.meshUpperTeeth.gameObject.GetComponent<Renderer>().sharedMaterial;
        from.meshLowerTeeth.gameObject.GetComponent<Renderer>().sharedMaterial = toThis.meshLowerTeeth.gameObject.GetComponent<Renderer>().sharedMaterial;

        OnTransformAvatarChildrenChanged(from, toThis);

        Debug.Log("Done assigning");
    }

    public void OnTransformAvatarChildrenChanged(AvatarInfo from, AvatarInfo toThis) {
        //Assigning skeleton locations to account for female/male differences
        Transform[] fromSkeleton = from.skeletonBase.GetComponentsInChildren<Transform>();
        Transform[] toSkeleton = toThis.skeletonBase.GetComponentsInChildren<Transform>();
        //from.skeletonBase.transform.localPosition = toThis.skeletonBase.transform.localPosition;
        from.avatarRoot.transform.localScale = toThis.skeletonBase.transform.localScale;
        for(int i = 0; i < fromSkeleton.Length; i++)
        {
            fromSkeleton[i].transform.localPosition = toSkeleton[i].transform.localPosition;
            
            //Debug.Log("Name from: " + fromSkeleton[i].name + "    To: " + toSkeleton[i].name);
            fromSkeleton[i].name = toSkeleton[i].name;
            
        }

        StartCoroutine(RebindThisAnim(from.anim));
    }

    public IEnumerator RebindThisAnim(Animator anim)
    {
        yield return new WaitForEndOfFrame();
        anim.Rebind();
        anim.applyRootMotion = true;
    }

    //function called by server to change the other players' character
    public void ChangeAvatar(AvatarInfo from, GenderList.genders gender)
    {
        AvatarInfo toThis;
        GameObject targetPrefab;

        switch(gender)
        {
            case GenderList.genders.Male1:
                targetPrefab = maleCharPrefab;
                break;
            case GenderList.genders.Female:
                targetPrefab = femaleCharPrefab;
                break;
            default:
                targetPrefab = defaultPrefab;
                break;
        }

        toThis = targetPrefab.GetComponent<AvatarInfo>();

        from.anim.avatar = toThis.anim.avatar;
        from.anim.runtimeAnimatorController = toThis.anim.runtimeAnimatorController;
        //from.meshHair.sharedMesh = toThis.meshHair.sharedMesh;
        //from.meshHead.sharedMesh = toThis.meshHead.sharedMesh;
        //from.meshChest.sharedMesh = toThis.meshChest.sharedMesh;
        //from.meshArm.sharedMesh = toThis.meshArm.sharedMesh;
        //from.meshForearm.sharedMesh = toThis.meshForearm.sharedMesh;
        //from.meshSpine.sharedMesh = toThis.meshSpine.sharedMesh;
        //from.meshPelvis.sharedMesh = toThis.meshPelvis.sharedMesh;
        //from.meshLegs.sharedMesh = toThis.meshLegs.sharedMesh;
        //from.meshFeet.sharedMesh = toThis.meshFeet.sharedMesh;
        //from.meshHat.sharedMesh = toThis.meshHat.sharedMesh;
        //from.meshShirt.sharedMesh = toThis.meshShirt.sharedMesh;
        //from.meshPants.sharedMesh = toThis.meshPants.sharedMesh;
        //from.meshShoes.sharedMesh = toThis.meshShoes.sharedMesh;
        //from.meshAccessories.sharedMesh = toThis.meshAccessories.sharedMesh;

        //Updated
        from.meshRightEye.sharedMesh = toThis.meshRightEye.sharedMesh;
        from.meshHead.sharedMesh = toThis.meshHead.sharedMesh;
        from.meshLeftEye.sharedMesh = toThis.meshLeftEye.sharedMesh;
        from.meshBody.sharedMesh = toThis.meshBody.sharedMesh;
        from.meshLowerTeeth.sharedMesh = toThis.meshLowerTeeth.sharedMesh;
        from.meshUpperTeeth.sharedMesh = toThis.meshUpperTeeth.sharedMesh;

        //visibility
        //from.meshHair.gameObject.SetActive(toThis.meshHair.gameObject.activeSelf);
        //from.meshHead.gameObject.SetActive(toThis.meshHead.gameObject.activeSelf);
        //from.meshChest.gameObject.SetActive(toThis.meshChest.gameObject.activeSelf);
        //from.meshArm.gameObject.SetActive(toThis.meshArm.gameObject.activeSelf);
        //from.meshForearm.gameObject.SetActive(toThis.meshForearm.gameObject.activeSelf);
        //from.meshSpine.gameObject.SetActive(toThis.meshSpine.gameObject.activeSelf);
        //from.meshPelvis.gameObject.SetActive(toThis.meshPelvis.gameObject.activeSelf);
        //from.meshLegs.gameObject.SetActive(toThis.meshLegs.gameObject.activeSelf);
        //from.meshFeet.gameObject.SetActive(toThis.meshFeet.gameObject.activeSelf);
        //from.meshHat.gameObject.SetActive(toThis.meshHat.gameObject.activeSelf);
        //from.meshShirt.gameObject.SetActive(toThis.meshShirt.gameObject.activeSelf);
        //from.meshPants.gameObject.SetActive(toThis.meshPants.gameObject.activeSelf);
        //from.meshShoes.gameObject.SetActive(toThis.meshShoes.gameObject.activeSelf);
        //from.meshAccessories.gameObject.SetActive(toThis.meshAccessories.gameObject.activeSelf);

        //Updated
        from.meshLeftEye.gameObject.SetActive(toThis.meshLeftEye.gameObject.activeSelf);
        from.meshHead.gameObject.SetActive(toThis.meshHead.gameObject.activeSelf);
        from.meshRightEye.gameObject.SetActive(toThis.meshRightEye.gameObject.activeSelf);
        from.meshLowerTeeth.gameObject.SetActive(toThis.meshLowerTeeth.gameObject.activeSelf);
        from.meshUpperTeeth.gameObject.SetActive(toThis.meshUpperTeeth.gameObject.activeSelf);
        from.meshBody.gameObject.SetActive(toThis.meshBody.gameObject.activeSelf);

        // get the materialfrom.
        //from.meshHair.gameObject.GetComponent<Renderer>().sharedMaterial = toThis.meshHair.gameObject.GetComponent<Renderer>().sharedMaterial;
        //from.meshHead.gameObject.GetComponent<Renderer>().sharedMaterial = toThis.meshHead.gameObject.GetComponent<Renderer>().sharedMaterial;
        //from.meshChest.gameObject.GetComponent<Renderer>().sharedMaterial = toThis.meshChest.gameObject.GetComponent<Renderer>().sharedMaterial;
        //from.meshArm.gameObject.GetComponent<Renderer>().sharedMaterial = toThis.meshArm.gameObject.GetComponent<Renderer>().sharedMaterial;
        //from.meshForearm.gameObject.GetComponent<Renderer>().sharedMaterial = toThis.meshForearm.gameObject.GetComponent<Renderer>().sharedMaterial;
        //from.meshSpine.gameObject.GetComponent<Renderer>().sharedMaterial = toThis.meshSpine.gameObject.GetComponent<Renderer>().sharedMaterial;
        //from.meshPelvis.gameObject.GetComponent<Renderer>().sharedMaterial = toThis.meshPelvis.gameObject.GetComponent<Renderer>().sharedMaterial;
        //from.meshLegs.gameObject.GetComponent<Renderer>().sharedMaterial = toThis.meshLegs.gameObject.GetComponent<Renderer>().sharedMaterial;
        //from.meshFeet.gameObject.GetComponent<Renderer>().sharedMaterial = toThis.meshFeet.gameObject.GetComponent<Renderer>().sharedMaterial;
        //from.meshHat.gameObject.GetComponent<Renderer>().sharedMaterial = toThis.meshHat.gameObject.GetComponent<Renderer>().sharedMaterial;
        //from.meshShirt.gameObject.GetComponent<Renderer>().sharedMaterial = toThis.meshShirt.gameObject.GetComponent<Renderer>().sharedMaterial;
        //from.meshPants.gameObject.GetComponent<Renderer>().sharedMaterial = toThis.meshPants.gameObject.GetComponent<Renderer>().sharedMaterial;
        //from.meshShoes.gameObject.GetComponent<Renderer>().sharedMaterial = toThis.meshShoes.gameObject.GetComponent<Renderer>().sharedMaterial;
        //from.meshAccessories.gameObject.GetComponent<Renderer>().sharedMaterial = toThis.meshAccessories.gameObject.GetComponent<Renderer>().sharedMaterial;

        //Updated
        from.meshRightEye.gameObject.GetComponent<Renderer>().sharedMaterial = toThis.meshRightEye.gameObject.GetComponent<Renderer>().sharedMaterial;
        from.meshHead.gameObject.GetComponent<Renderer>().sharedMaterial = toThis.meshHead.gameObject.GetComponent<Renderer>().sharedMaterial;
        from.meshLeftEye.gameObject.GetComponent<Renderer>().sharedMaterial = toThis.meshLeftEye.gameObject.GetComponent<Renderer>().sharedMaterial;
        from.meshBody.gameObject.GetComponent<Renderer>().sharedMaterial = toThis.meshBody.gameObject.GetComponent<Renderer>().sharedMaterial;
        from.meshLowerTeeth.gameObject.GetComponent<Renderer>().sharedMaterial = toThis.meshLowerTeeth.gameObject.GetComponent<Renderer>().sharedMaterial;
        from.meshUpperTeeth.gameObject.GetComponent<Renderer>().sharedMaterial = toThis.meshUpperTeeth.gameObject.GetComponent<Renderer>().sharedMaterial;

        OnTransformAvatarChildrenChanged(from, toThis);
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
                if(pv.GetComponent<AttachAvatar>().avatarBodyLocation.GetComponent<AvatarInfo>().meshHead.sharedMesh == 
                    defaultPrefab.GetComponent<AvatarInfo>().meshHead.sharedMesh)
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
                if( pv.GetComponent<AttachAvatar>().avatarBodyLocation.GetComponent<AvatarInfo>().meshHead.sharedMesh == 
                    defaultPrefab.GetComponent<AvatarInfo>().meshHead.sharedMesh)
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
