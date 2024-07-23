using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIManager : SerializedMonoBehaviour
{
    public static UIManager Instance;

    public Dictionary<CanvasTypes, GameObject> allCanvases;

    [HideInInspector] public NextLevelPanel nextLevelPanel;
    [HideInInspector] public FailedPanel failedPanel;
    [HideInInspector] public LevelProgressUIController levelProgressUIController;
    [HideInInspector] public ClearSlotHint clearSlotHint;
    [HideInInspector] public ChangeSlotHint changeSlotHint;

    private void Awake()
    {
        Instance = this;
        nextLevelPanel= GetComponentInChildren<NextLevelPanel>(true);
        failedPanel = GetComponentInChildren<FailedPanel>(true);
        levelProgressUIController= GetComponentInChildren<LevelProgressUIController>(true);
        clearSlotHint = GetComponentInChildren<ClearSlotHint>(true);
        changeSlotHint = GetComponentInChildren<ChangeSlotHint>(true);
    }


    private void OnEnable()
    {
        EventManager.UIEvents.CanvasSetter += CanvasSetter;
    }
    private void OnDisable()
    {
        EventManager.UIEvents.CanvasSetter -= CanvasSetter;
    }
    public void CanvasSetter(CanvasTypes canvasType, bool openOrClose)
    {
        if (allCanvases.ContainsKey(canvasType))
        {
            allCanvases[canvasType].gameObject.SetActive(openOrClose);
        }
    }
}
