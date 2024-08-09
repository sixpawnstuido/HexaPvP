using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using static JuiceTargetUIElement;

[CreateAssetMenu(fileName ="VisualData", menuName = "VisualData")]
public class VisualData : SerializedScriptableObject
{
    public Dictionary<VFXType,GameObject> VFXData;
    public Dictionary<PrefabType,GameObject> prefabData;
    public Dictionary<HexagonTypes,HexagonElement> hexagons;
    
    public List<Sprite> oppIcons; 
    public List<Sprite> oppFlagIcons; 
    public List<string> oppNames; 
    public enum VFXType
    {
        Confetti,
    }
    public enum PrefabType
    {
      GridHolder,
      HexagonHolder,
      Knife,
      TargetUI
    }
}
