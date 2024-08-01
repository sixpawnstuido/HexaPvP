using System;
using System.Collections;
using System.Collections.Generic;
using System.Security;
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

    public Image HeartImage => heartImage;
    [SerializeField] private Image heartImage;

    [SerializeField] private Transform heartParent;

    [ReadOnly] public int currentHealth;
    [ReadOnly] public int totalHealth;

    [SerializeField] private Color32 greyColor;
    [SerializeField] private Color32 originalColor;

    private PvPController _pvPController;

    [SerializeField] private ParticleSystem trailArrivedVFX;

    private MinusTextPool _minusTextPool;

    [SerializeField] private Transform minusTextStartPos;
    private void Awake()
    {
        _pvPController = GetComponentInParent<PvPController>();
        _minusTextPool = GetComponentInChildren<MinusTextPool>();
    }

    public void DecreaseHealth(int hexagonElementAmount,int comboStage=1)
    {
        comboStage = Mathf.Max(comboStage, 1);
        currentHealth-= hexagonElementAmount;
        currentHealth = Mathf.Max(currentHealth, 0);
        SetFillAmount();
        SetHealthText();
        if (currentHealth == 0)
        {
            _pvPController.LevelCompleted(playerType);
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


    public void TrailArrivedState(int amount,int comboStage=1)
    {
        if (currentHealth <= 0) return;
        trailArrivedVFX.Stop();
        trailArrivedVFX.Play();
        var minusText = _minusTextPool.GetParticle();
        minusText.Init(amount,minusTextStartPos.position,comboStage);
        if (!DOTween.IsTweening(transform.GetHashCode()))
        {
            int multiplier = Mathf.Min(amount,10);
            heartParent 
                .DOPunchPosition(new Vector3(0f, (2f*multiplier), 0), .3f, 10) 
                .SetId(transform.GetHashCode());
        }

        if (playerType==PlayerType.OPPONENT)
        {
            AudioManager.Instance.Play(AudioManager.AudioEnums.TargetCompleted);
        }
        else
        {
            AudioManager.Instance.Play(AudioManager.AudioEnums.Cut3);
        }
    }
}