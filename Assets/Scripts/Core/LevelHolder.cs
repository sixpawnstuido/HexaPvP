using System;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening.Core;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class LevelHolder : MonoBehaviour
{
    [ReadOnly] public GridHolderController gridController;
    [ReadOnly] public HexagonHolderController hexagonHolderController;
    [ReadOnly] public HexagonSpawner hexagonSpawner;

    [SerializeField] private List<BoxCollider> slots;
    [SerializeField] private GameObject fullFruitTutorial;

    private void Awake()
    {
        gridController = GetComponentInChildren<GridHolderController>();
        hexagonHolderController = GetComponentInChildren<HexagonHolderController>();
        hexagonSpawner = GetComponentInChildren<HexagonSpawner>();
    }

    private void OnEnable()
    {
        // bool isLevelCountGreaterThenOne = LevelManager.Instance.LevelCount > 1;
        // MetaProgress.Instance.HolderState(isLevelCountGreaterThenOne);
    }

    private void Start()
    {
        TutorialManager.Instance.slots = this.slots;
        TutorialManager.Instance.fullFruitTutorial = this.fullFruitTutorial;
    }
}