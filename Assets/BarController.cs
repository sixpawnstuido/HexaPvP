using System;
using System.Collections;
using System.Collections.Generic;
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

    public void ChangeProgress(PlayerType playerType,int hexagonAmount)
    {
        if (playerType==PlayerType.PLAYER)
        {
            float increaseAmount = Mathf.InverseLerp(0, targetAmount, hexagonAmount);
            playerImage.fillAmount += increaseAmount;
            opponentImage.fillAmount -= increaseAmount;
            if (playerImage.fillAmount>=1)
            {
                LevelManager.Instance.OpenNextLevelPanel();
            }
        }
        else
        {
            float increaseAmount = Mathf.InverseLerp(0, targetAmount, hexagonAmount);
            opponentImage.fillAmount += increaseAmount;
            playerImage.fillAmount -= increaseAmount;
            if (opponentImage.fillAmount>=1)
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
