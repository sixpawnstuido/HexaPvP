using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class BarController : SerializedMonoBehaviour
{
    public static BarController Instance;
    
    public Dictionary<PlayerType, NewTarget> targets;

    public Image playerImage;
    public Image opponentImage;

    public int targetAmount;

    private void Awake()
    {
        Instance = this;
    }

    [Button]
    public void ChangeProgress(PlayerType playerType,int hexagonAmount)
    {
        if (playerType==PlayerType.PLAYER)
        {
            float increaseAmount = Mathf.InverseLerp(0, targetAmount, hexagonAmount);
            var fillAmount= playerImage.fillAmount + increaseAmount;
            var fillAmountOpponent= opponentImage.fillAmount - increaseAmount;
            // opponentImage.fillAmount -= increaseAmount;

            DOVirtual.Float(playerImage.fillAmount, fillAmount, .3f, (v) => playerImage.fillAmount = v);
            DOVirtual.Float(opponentImage.fillAmount, fillAmountOpponent, .3f, (v) => opponentImage.fillAmount = v);
            
            if (fillAmount>=1)
            {
                LevelManager.Instance.OpenNextLevelPanel();
            }
        }
        else
        {
            float increaseAmount = Mathf.InverseLerp(0, targetAmount, hexagonAmount);
            
            var fillAmount= opponentImage.fillAmount + increaseAmount;
            var fillAmountOpponent= playerImage.fillAmount - increaseAmount;
            
            DOVirtual.Float(playerImage.fillAmount, fillAmountOpponent, .2f, (v) => playerImage.fillAmount = v);
            DOVirtual.Float(opponentImage.fillAmount, fillAmount, .2f, (v) => opponentImage.fillAmount = v);
            if (fillAmount>=1)
            {
                LevelManager.Instance.OpenFailedPanel();
            }
        }

        if (playerImage.fillAmount>opponentImage.fillAmount)
        {
            playerImage.transform.SetSiblingIndex(0);
        }
        else
        {
            opponentImage.transform.SetSiblingIndex(0);
        }
    }
    
}
