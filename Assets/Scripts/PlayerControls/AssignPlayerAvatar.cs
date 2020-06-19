using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssignPlayerAvatar : MonoBehaviour
{
    private GenderList.genders gender;
    public GameObject malePrefab;
    public GameObject femalePrefab;
    public GameObject defaultPrefab;
    public GameObject selectedPrefab;

    void Awake()
    {
        DontDestroyOnLoad(this);
        selectedPrefab = defaultPrefab;
    }
    public GenderList.genders Gender {
        get{ 
            return this.gender; 
        }
        set{ 
            gender = value; 
            switch(gender)
            {
                case GenderList.genders.Male:
                    selectedPrefab = malePrefab;
                    break;
                case GenderList.genders.Female:
                    selectedPrefab = femalePrefab;
                    break;
                case GenderList.genders.NonBinary:
                    break;
                default:
                    break;
            }
        }
    }

}
