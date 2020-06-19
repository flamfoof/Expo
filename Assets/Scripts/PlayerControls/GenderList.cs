using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenderList : MonoBehaviour
{
    public enum genders{
        None,
        Male,
        Female,
        NonBinary
    };

    public genders gender;
}
