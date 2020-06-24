using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenderList : MonoBehaviour
{
    public enum genders{
        None,
        Male1,
        Male2,
        Female1,
        Female2,
        NonBinary
    };

    public genders gender;
}
