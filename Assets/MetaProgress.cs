using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class MetaProgress : SerializedMonoBehaviour
{
    public static MetaProgress Instance;

    [SerializeField] private Transform _holderState;
    [SerializeField] private Transform _target;
    [SerializeField] private Transform _checkmark;
    
    [SerializeField] private SlicedFilledImage _fillImage;

    [SerializeField] private TextMeshProUGUI _fruitAmountText;


    [SerializeField] private Dictionary<HexagonTypes, Sprite> _fruitSprites;

    [SerializeField] private Image _refFruitImage;


    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        ProgressUpdate();
    }


    public void HolderState(bool state)
    {
        _holderState.gameObject.SetActive(state);
    }
    public void ResetProgress()
    {
        _fillImage.fillAmount = 0;
        _fruitAmountText.SetText("0");
        _fruitAmountText.transform.DOScale(1, 0);
        _checkmark.DOScale(0, 0);
    }

    public void ProgressUpdate()
    {
        int sortedFruitCountForMeta = LevelManager.Instance.SortedFruitCount;
        _fillImage.fillAmount = Mathf.InverseLerp(0, 30, sortedFruitCountForMeta);
        _fruitAmountText.SetText($"{sortedFruitCountForMeta}");
        if (sortedFruitCountForMeta>=30)
        {
            _fruitAmountText.transform.DOScale(0, .2f);
            _checkmark.DOScale(1, .2f).SetEase(Ease.OutBack);
        }
    }

    public void FruitMoveToTarget(Vector3 spawnPos,HexagonTypes fruitType)
    {
        if (!_fruitSprites.ContainsKey(fruitType))  return;
        
        var refFruit = Instantiate(_refFruitImage, _target);
        refFruit.transform.position = spawnPos;
        refFruit.sprite = _fruitSprites[fruitType];
        
        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(0.15f);
        seq.Append(refFruit.transform.DOScale(1,.3f).SetEase(Ease.OutBack));
        seq.AppendInterval(0.2f);
        seq.Append(refFruit.transform.DOMove(transform.position, .5f).SetEase(Ease.InBack));
        seq.Join(refFruit.transform.DOScale(.8f,.5f));
        seq.AppendCallback(()=>
        {
            if (!DOTween.IsTweening(_target.GetHashCode()))
            {
                _target
                    .DOPunchScale(new Vector3(-.1f, 0.1f, 0), .5f, 10)
                    .SetId(_target.GetHashCode());   
            }
            AudioManager.Instance.Play(AudioManager.AudioEnums.MetaProgress);
            ProgressUpdate();
            Destroy(refFruit.gameObject);
        });
    }
}
