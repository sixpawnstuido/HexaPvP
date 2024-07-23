using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class BlenderReset : MonoBehaviour
{
    [SerializeField] private Button _resetButton;

    [SerializeField] private GameObject _disabledButton;

    [SerializeField] private int _resetCost;

    private BlenderElement _blenderElement;

    public static Action UpdateButton;

    [SerializeField] private Transform _holder;

    [Header("HandTut")]
    [SerializeField] private bool _isTut;
    [ShowIf("_isTut")] public GameObject handTut;
    [HideIf("_isTut")] [SerializeField] private BlenderReset _firstBlender;

    [SerializeField] private ParticleSystem _blenderResetVFX;

    public int IsTutCompleted
    {
        get => PlayerPrefs.GetInt("IsBlenderResetTutCompleted" + gameObject.name, 0);
        set => PlayerPrefs.SetInt("IsBlenderResetTutCompleted" + gameObject.name, value);
    }

    private void Awake()
    {
        _resetButton.onClick.AddListener(OnResetButtonClick);
        _blenderElement = GetComponent<BlenderElement>();
    }

    private void OnEnable()
    {
       // UpdateButton += UpdateButtonState;
    }

    private void OnDisable()
    {
       // UpdateButton -= UpdateButtonState;
    }

    private void Start()
    {
     //UpdateButtonState();
    }

    private void UpdateButtonState()
    {
        if (CurrencyManager.Instance.GoldAmount >= _resetCost)
        {
            _disabledButton.SetActive(false);
        }
        else
        {
            _disabledButton.SetActive(true);
        }
    }

    public void ButtonScaleState()
    {
        if (LevelManager.Instance.LevelCount == 1) return;
        if (_blenderElement.juiceLevel > 0)
        {
            if (LevelManager.Instance.LevelCount == 2)
            {
                _holder.DOScale(1, .2f).SetEase(Ease.OutBack);
                if (IsTutCompleted == 0 && _isTut)
                {
                    handTut.SetActive(true);
                }
            }
            else
            {
                _holder.DOScale(1, .2f).SetEase(Ease.OutBack);
            }
        }
        else
        {
            _holder.DOScale(0, .2f).SetEase(Ease.Flash);
        }
    }

    private void OnResetButtonClick()
    {
        if (CurrencyManager.Instance.GoldAmount >= _resetCost && !_blenderElement.isProcessing &&
            _blenderElement.juiceLevel != 4)
        {
            _blenderElement.isProcessing = false;
            CurrencyManager.Instance.TakeGold(_resetCost);
            _blenderElement.ModelAnim.SetTrigger("Shake");
            _blenderElement.ResetBlender(1f);
            DOVirtual.DelayedCall(1f, () => _blenderElement.isProcessing = true);
            AudioManager.Instance.Play(AudioManager.AudioEnums.BlenderReset);
            _blenderResetVFX.Stop();
            _blenderResetVFX.Play();
            if (IsTutCompleted == 0)
            {
                if (_isTut)
                {
                    handTut.SetActive(false);
                    IsTutCompleted = 1;
                }
                else
                {
                    _firstBlender.handTut.SetActive(false);
                    _firstBlender.IsTutCompleted = 1;
                }
            }
        }
    }

    public void ResetBlenderAnim()
    {
        
    }
}