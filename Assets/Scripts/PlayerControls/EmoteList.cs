using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmoteList : MonoBehaviour
{
    [SerializeField]
    public List<GameObject> emotesList = new List<GameObject>();
    public static string emotePath = "Emotes/";
}
