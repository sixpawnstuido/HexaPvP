using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Unity.Mathematics;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance;

    public int clearSliceAmount;
    public int clearJuiceAmount;

    [ShowInInspector, ReadOnly]
    public int GoldAmount
    {
        get => PlayerPrefs.GetInt("GoldAmount", 20);
        set => PlayerPrefs.SetInt("GoldAmount", value);
    }

    private void Awake()
    {
        Instance = this;
    }

    [Button]
    public void AddGold(int goldAmount)
    {
        GoldAmount += goldAmount;
        GoldPanel.Instance.UpdateGoldText(GoldAmount);
        if (EventManager.CoreEvents.CheckIfGridsUnlockable != null) EventManager.CoreEvents.CheckIfGridsUnlockable();
    }

    [Button]
    public void TakeGold(int goldAmount)
    {
        GoldAmount -= goldAmount;
        GoldAmount =(int) Mathf.Clamp(GoldAmount,0, float.MaxValue);
        GoldPanel.Instance.UpdateGoldText(GoldAmount);
        if (EventManager.CoreEvents.CheckIfGridsUnlockable != null) EventManager.CoreEvents.CheckIfGridsUnlockable();
    }
}