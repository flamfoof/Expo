using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarInfo : MonoBehaviour
{
    public Animator anim;
    //public SkinnedMeshRenderer meshHair;
    //public SkinnedMeshRenderer meshHead;
    //public SkinnedMeshRenderer meshChest;
    //public SkinnedMeshRenderer meshArm;
    //public SkinnedMeshRenderer meshForearm;
    //public SkinnedMeshRenderer meshSpine;
    //public SkinnedMeshRenderer meshPelvis;
    //public SkinnedMeshRenderer meshLegs;
    //public SkinnedMeshRenderer meshFeet;

    //public SkinnedMeshRenderer meshHat;
    //public SkinnedMeshRenderer meshShirt;
    //public SkinnedMeshRenderer meshPants;
    //public SkinnedMeshRenderer meshShoes;
    //public SkinnedMeshRenderer meshAccessories; 
    [Header("Character Skinned Mesh")]
    public SkinnedMeshRenderer meshLeftEye;
    public SkinnedMeshRenderer meshRightEye;
    public SkinnedMeshRenderer meshBody;
    public SkinnedMeshRenderer meshHead;
    public SkinnedMeshRenderer meshUpperTeeth;
    public SkinnedMeshRenderer meshLowerTeeth;
    public GameObject skeletonBase;
    public GameObject avatarRoot;
}
