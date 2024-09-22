using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Wave", menuName = "Data/Waves")]
[Serializable]
public class Wave : ScriptableObject
{
    public GameObject[] Characters;
}
