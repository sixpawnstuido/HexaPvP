using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class AvatarElement : MonoBehaviour
{
    public PlayerType playerType;

    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI nameText;

    [SerializeField] private Image heartImage;

    [ReadOnly] public int currentHealth;
    [ReadOnly] public int totalHealth;

    [SerializeField] private Color32 greyColor;
    [SerializeField] private Color32 originalColor;
    

    public void DecreaseHealth()
    {
        currentHealth--;
        SetFillAmount();
        SetHealthText();
        if (currentHealth <= 0)
        {
            FailState();
        }
    }

    [Button]
    public void SetFillAmount()
    {
        var targetFillAmount = Mathf.InverseLerp(0, totalHealth, currentHealth);
        
        heartImage.fillAmount = targetFillAmount;
    }

    public void SetTargetAmount(int goal)
    {
        totalHealth = goal;
        currentHealth = totalHealth;
    }

    public void SetHealthText() => healthText.SetText($"{currentHealth}");

    public void SetColor(bool isGrey)
    {
        if (isGrey)
        {
            heartImage.color=greyColor;
        }
        else
        {
            heartImage.color=originalColor;
        }
    }


    private void FailState()
    {
        if (playerType == PlayerType.PLAYER)
        {
            
        }
        else
        {
            
        }
    }
}