using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BlenderGhost : MonoBehaviour
{
    [SerializeField] private Button buyButton;

    [SerializeField] private BlenderElement thirdBlender;

    public int price;

    [SerializeField] private TextMeshProUGUI priceText;

    private BlenderController _blenderController;

    private void Awake()
    {
        _blenderController = GetComponentInParent<BlenderController>();
    }

    private void Start()
    {
        buyButton.onClick.AddListener(OnBuyClicked);

        priceText.text = price.ToString();
    }

    private void OnBuyClicked()
    {
        if (CurrencyManager.Instance.GoldAmount < price) return;

        thirdBlender.gameObject.SetActive(true);

        thirdBlender.transform.position = transform.position;

        thirdBlender.transform.DOPunchScale(.5f * Vector3.one, .25f, 3);
     //   thirdBlender.transform.DOJump(thirdBlender.transform.position,2,1,.5f);

        thirdBlender.RegisterItselfToBlenderController();

        CurrencyManager.Instance.TakeGold(price);
        


        _blenderController.isThirdBlenderOpened = true;
        
        
        gameObject.SetActive(false);

        EventHandler.Instance.LogEconomyEvent(price, 0, "ThirdBlender");

    }
}
