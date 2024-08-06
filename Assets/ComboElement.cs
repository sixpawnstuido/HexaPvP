using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ComboElement : MonoBehaviour
{
    [SerializeField] private Transform _comboImage;
    [SerializeField] private List<Transform> _comboAmountList;
    [SerializeField] private List<Transform> _comboTextList;


    private Vector3 _comboImageFirstPos;
    private Vector3 _comboAmountFirstPos;

    [SerializeField] private ParticleSystem _VFX;
    [SerializeField] private ParticleSystem _VFX2;

    private void Awake()
    {
        _comboImageFirstPos = _comboTextList[0].localPosition;
        _comboAmountFirstPos = _comboAmountList[0].localPosition;
    }

    public void ResetComboElement()
    {
        //_comboImage.localPosition = _comboImageFirstPos;
        //_comboImage.localScale = Vector3.zero;
        for (int i = 0; i < _comboTextList.Count; i++)
        {
            _comboTextList[i].localPosition= _comboImageFirstPos;
            _comboTextList[i].localScale = Vector3.zero;
        }

        for (int i = 0; i < _comboAmountList.Count; i++)
        {
            _comboAmountList[i].localPosition = _comboAmountFirstPos;
            _comboAmountList[i].localScale = Vector3.zero;
        }
    }

    public void ComboAnim(int comboStage, Vector3 startPos)
    {
        ResetComboElement();
      //  transform.position = startPos;
        AudioManager.Instance.Play(AudioManager.AudioEnums.Combo);

        int comboStageClampted=Math.Min(comboStage, _comboTextList.Count-1);
        _VFX2.Stop();
        _VFX2.Play();

        Sequence seq = DOTween.Sequence();
        seq.Append(_comboTextList[comboStageClampted].DOScale(Vector3.one, .2f).SetEase(Ease.OutBack));
        seq.AppendInterval(0.05f);
        seq.Append(_comboAmountList[comboStageClampted].DOScale(Vector3.one, .2f).SetEase(Ease.OutBack));

        seq.Append(_comboTextList[comboStageClampted].DOScale(Vector3.zero, .2f).SetEase(Ease.InBack));
        seq.AppendInterval(0.01f);
        seq.Append(_comboAmountList[comboStageClampted].DOScale(Vector3.zero, .2f).SetEase(Ease.InBack));


        //seq.Append(_comboTextList[comboStageClampted].DOMove(Vector3.up / 2, .2f).SetRelative().SetEase(Ease.Flash));
        //seq.Join(_comboAmountList[comboStageClampted].DOMove(Vector3.down / 2, .2f).SetRelative().SetEase(Ease.Flash));
        //seq.Append(_comboTextList[comboStageClampted].DOMove(Vector3.down / 2, .1f).SetRelative().SetEase(Ease.Linear));
        //seq.Join(_comboAmountList[comboStageClampted].DOMove(Vector3.up / 2, .1f).SetRelative().SetEase(Ease.Linear));

        //seq.Append(_comboTextList[comboStageClampted].DOScale(0, .1f));
        //seq.Join(_comboAmountList[comboStageClampted].DOScale(Vector3.zero, .1f));
        //  seq.Join(SpawnMoneyTween(comboStageClampted));
        seq.AppendInterval(.2f);
        seq.AppendCallback(()=>gameObject.SetActive(false));
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