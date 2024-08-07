using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.UI;

public class BarController : SerializedMonoBehaviour
{
    public static BarController Instance;
    
    public Dictionary<PlayerType, NewTarget> targets;

    public Image playerImage;
    public Image opponentImage;

    public int targetAmount;

    [SerializeField] private Transform holder;

    public bool isLevelEnd;

    private void Awake()
    {
        Instance = this;
    }

    public void ChangeProgress(PlayerType playerType,int hexagonAmount)
    {
        if (isLevelEnd) return;
        if (!DOTween.IsTweening(transform.GetHashCode()))
        {
            holder.DOPunchScale(new Vector3(-.05f, 0.05f, 0), .3f, 5) .SetId(transform.GetHashCode());
        }
        AudioManager.Instance.Play(AudioManager.AudioEnums.BarSound);
        if (playerType==PlayerType.PLAYER)
        {
            float increaseAmount = Mathf.InverseLerp(0, targetAmount, hexagonAmount);
            var fillAmount= playerImage.fillAmount + (increaseAmount*2);
            var fillAmountOpponent= opponentImage.fillAmount - (increaseAmount*2);

            DOVirtual.Float(playerImage.fillAmount, fillAmount, .3f, (v) => playerImage.fillAmount = v);
            DOVirtual.Float(opponentImage.fillAmount, fillAmountOpponent, .3f, (v) => opponentImage.fillAmount = v);
            
            if (fillAmount>=.95f)
            {
                LevelManager.Instance.OpenNextLevelPanel();
                OrderOfPlayPanel.Instance.gameObject.SetActive(false);
                isLevelEnd = true;
            }
            targets[PlayerType.PLAYER].splashVFX.Stop();
            targets[PlayerType.PLAYER].splashVFX.Play();
        }
        else
        {
            float increaseAmount = Mathf.InverseLerp(0, targetAmount, hexagonAmount);
            
            var fillAmount= opponentImage.fillAmount +(increaseAmount*2);
            var fillAmountOpponent= playerImage.fillAmount -(increaseAmount*2);
            
            DOVirtual.Float(playerImage.fillAmount, fillAmountOpponent, .2f, (v) => playerImage.fillAmount = v);
            DOVirtual.Float(opponentImage.fillAmount, fillAmount, .2f, (v) => opponentImage.fillAmount = v);
            if (fillAmount>=.95f)
            {
                LevelManager.Instance.OpenFailedPanel();
                OrderOfPlayPanel.Instance.gameObject.SetActive(false);
                isLevelEnd = true;
            }
            
            targets[PlayerType.OPPONENT].splashVFX.Stop();
            targets[PlayerType.OPPONENT].splashVFX.Play();
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

    public void ResetBar()
    {
        opponentImage.fillAmount = .5f;
        playerImage.fillAmount = .5f;
    }
    
}
