using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ComboElement : MonoBehaviour
{
    [SerializeField] private Transform _comboImage;
    [SerializeField] private List<Transform> _comboAmountList;


    private Vector3 _comboImageFirstPos;
    private Vector3 _comboAmountFirstPos;

    [SerializeField] private ParticleSystem _VFX;

    private void Awake()
    {
        _comboImageFirstPos = _comboImage.localPosition;
        _comboAmountFirstPos = _comboAmountList[0].localPosition;
    }

    public void ResetComboElement()
    {
        _comboImage.localPosition = _comboImageFirstPos;
        _comboImage.localScale = Vector3.zero;
        for (int i = 0; i < _comboAmountList.Count; i++)
        {
            _comboAmountList[i].localPosition = _comboAmountFirstPos;
            _comboAmountList[i].localScale = Vector3.zero;
        }
    }

    public void ComboAnim(int comboStage, Vector3 startPos)
    {
        ResetComboElement();
        transform.position = startPos;
        gameObject.SetActive(true);
        AudioManager.Instance.Play(AudioManager.AudioEnums.Combo);

        Sequence seq = DOTween.Sequence();
        seq.Append(_comboImage.DOScale(Vector3.one, .2f).SetEase(Ease.OutBack));
        seq.AppendInterval(0.1f);
        seq.Append(_comboAmountList[comboStage].DOScale(Vector3.one, .2f).SetEase(Ease.OutBack));

        seq.Append(_comboImage.DOMove(Vector3.up / 2, .2f).SetRelative().SetEase(Ease.Flash));
        seq.Join(_comboAmountList[comboStage].DOMove(Vector3.down / 2, .2f).SetRelative().SetEase(Ease.Flash));
        seq.Append(_comboImage.DOMove(Vector3.down / 2, .1f).SetRelative().SetEase(Ease.Linear));
        seq.Join(_comboAmountList[comboStage].DOMove(Vector3.up / 2, .1f).SetRelative().SetEase(Ease.Linear));

        seq.Append(_comboImage.DOScale(0, .1f));
        seq.Join(_comboAmountList[comboStage].DOScale(Vector3.zero, .1f));
        seq.Join(SpawnMoneyTween(comboStage));
    }

    private Tween SpawnMoneyTween(int comboStage)
    {
        return DOVirtual.DelayedCall(0, () =>
        {
            _VFX.Stop();
            _VFX.Play();
           // GoldPanel.Instance.ActivateGoldAnim(_VFX.transform.position - Vector3.up*1.5f, comboStage + 2);
        });
    }
}