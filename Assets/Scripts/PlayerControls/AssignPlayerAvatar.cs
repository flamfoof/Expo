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
        this.selectedPrefab = defaultPrefab;
    }
    public GenderList.genders Gender {
        get{ 
            return this.gender; 
        }
        set{ 
            gender = value; 
            switch(gender)
            {
                case GenderList.genders.Male1:
                    this.selectedPrefab = malePrefab;
                    break;
                case GenderList.genders.Female1:
                    this.selectedPrefab = femalePrefab;
                    break;
                case GenderList.genders.Male2:
                    this.selectedPrefab = malePrefab;
                    break;
                case GenderList.genders.Female2:
                    this.selectedPrefab = femalePrefab;
                    break;
                case GenderList.genders.NonBinary:
                    break;
                default:
                    break;
            }
        }
    }

}
