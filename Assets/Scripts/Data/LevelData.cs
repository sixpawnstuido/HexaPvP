using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="LevelData",menuName ="LevelData")]
public class LevelData : SerializedScriptableObject
{
    public Dictionary<int, LevelHolder> allLevels;
}
