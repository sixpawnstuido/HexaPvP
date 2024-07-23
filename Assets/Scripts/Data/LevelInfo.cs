using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="LevelInfo",menuName ="LevelInfo")]
public class LevelInfo : SerializedScriptableObject
{
    public Dictionary<int, LevelInfoValues> levelInfoValues;
    public class LevelInfoValues
    {
        [Header("DesiredHexagonAmount")]
        public int desiredHexagonAmount;

        [Header("SpawnCount")]
        public int maxHexagonCountIndividual;
        public int maxHexagonTypeCountIndividual;
        public int newTypeMinIndex;

        [Header("HexagonPositions")]
        public float hexagonElementFirstLocalPosY;
        public float hexagonElementYOffset;


        [Header("HexagonType")]
        public List<int> hexagonTypeStageCount=new();
        public List<HexagonTypes> hexagonTypesAtTheBeginning=new();
        public List<HexagonTypes> hexagonTypesInLine=new();
    }
}
