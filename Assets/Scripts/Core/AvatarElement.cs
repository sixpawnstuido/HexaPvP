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
    private void Awake()
    {
        _pvPController = GetComponentInParent<PvPController>();
    }

    public void DecreaseHealth(int hexagonElementAmount)
    {
        currentHealth-= hexagonElementAmount;
        currentHealth = Mathf.Max(currentHealth, 0);
        SetFillAmount();
        SetHealthText();
        if (currentHealth == 0)
        {
            _pvPController.LevelCompleted();
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


    public void TrailArrivedState()
    {
        trailArrivedVFX.Stop();
        trailArrivedVFX.Play();
        if (!DOTween.IsTweening(transform.GetHashCode()))
        {
            heartParent
                .DOPunchScale(new Vector3(-.1f, 0.1f, 0), .3f, 10) 
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