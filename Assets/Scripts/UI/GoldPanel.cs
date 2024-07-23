using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class GoldPanel : MonoBehaviour
{
    public static GoldPanel Instance;

    private CoinAnimation _coinAnimation;

    [SerializeField] private TextMeshProUGUI _goldText;

    private void Awake()
    {
        Instance = this;
        _coinAnimation = GetComponentInChildren<CoinAnimation>();
    }

    private void Start()
    {
        UpdateGoldText(CurrencyManager.Instance.GoldAmount);
    }

    [Button]
    public void ActivateGoldAnim(Vector3 target,int amount) => _coinAnimation.StartCoinAnimation(target,amount);

    public void UpdateGoldText(int goldAmount) => _goldText.SetText($"{goldAmount}");
}
