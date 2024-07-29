using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HintBuy : MonoBehaviour
{
    public Button button;
    [SerializeField] private Button _rwButton;

    public HintBase HintBase;

    public float hintPrice;

    public TextMeshProUGUI priceText;

    public HintBuyAnim HintBuyAnim;

    private void Awake()
    {
        _rwButton.onClick.AddListener(OnRwButtonClicked);
    }

    private void OnEnable()
    {
        if (CurrencyManager.Instance.GoldAmount >= hintPrice)
        {
            _rwButton.gameObject.SetActive(false);
            button.gameObject.SetActive(true);
        }
        else
        {
            _rwButton.gameObject.SetActive(true);
            button.gameObject.SetActive(false);
        }

        priceText.text = hintPrice.ToString();
    }

    private void Start()
    {
        button.onClick.AddListener(OnBuyClicked);
    }

    private void OnBuyClicked()
    {
        CurrencyManager.Instance.TakeGold((int)hintPrice);
        HintBuyAnim.PlayAnim();
        gameObject.SetActive(false);
     //   EventHandler.Instance.LogEconomyEvent((int)hintPrice, 0, HintBase.hintType.ToString());
    }

    private void OnRwButtonClicked()
    {
        HintBuyAnim.PlayAnim();
        gameObject.SetActive(false);
       // EventHandler.Instance.LogEconomyEvent((int)hintPrice, 0, HintBase.hintType.ToString());
    }
}
