using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public class MergeVFXController : SerializedMonoBehaviour
{
    public static MergeVFXController Instance;
    
    [SerializeField] private List<ParticleSystem> _mergeVFXList;
    [SerializeField] private ParticleSystem _mergeVFX;

    [SerializeField] private Dictionary<HexagonTypes, ParticleSystem.MinMaxGradient> _mergeVFXColorList;

    private void Awake()
    {
        Instance = this;
    }

    private ParticleSystem ReturnMergeVFX()
    {
        var mergeVFX = _mergeVFXList.FirstOrDefault(g=>!g.gameObject.activeInHierarchy);
        if (mergeVFX is null)
        {
            var instantiatedMergeVFX=Instantiate(_mergeVFX);
            if(!_mergeVFXList.Contains(instantiatedMergeVFX)) _mergeVFXList.Add(instantiatedMergeVFX);
            return instantiatedMergeVFX;
        }
        else
        {
            return mergeVFX;
        }
    }

    public void ActivateMergeVFX(Vector3 spawnPos,HexagonTypes fruitType)
    {
        var mergeVFX = ReturnMergeVFX();
        mergeVFX.transform.position = spawnPos;
        mergeVFX.gameObject.SetActive(true);
        var mergeVFXMain = mergeVFX.main;
        var mergeVFXMainChild = mergeVFX.GetComponentInChildren<ParticleSystem>().main;
        mergeVFXMain.startColor = _mergeVFXColorList[fruitType];
        mergeVFXMainChild.startColor = _mergeVFXColorList[fruitType];
    }
    
    
}
