using System;
using DG.Tweening;
using FIMSpace.Basics;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LockController : MonoBehaviour
{

    [SerializeField] private GameObject _holder;

    private GridHolder _gridHolder;

    [SerializeField] private int _lockUnlockAmount;

    private TextMeshProUGUI _lockAmountText;
    
   [SerializeField] private GameObject _rewardedImage;
   [SerializeField] private GameObject _coinImage;

    private Animator _animator;

    public Color32 green;
    public Color32 grey;

    private MaterialPropertyBlock mbp;

    [SerializeField] private MeshRenderer mesh;

    public int IsLockedOpened
    {
        get => PlayerPrefs.GetInt(transform.parent.name + LevelManager.Instance.LevelCount + "IsLockOpened", 0);
        set => PlayerPrefs.SetInt(transform.parent.name + LevelManager.Instance.LevelCount + "IsLockOpened", value);
    }

    [SerializeField] private bool _isLockRewarded;
    private Collider _collider;

    private void Awake()
    {
        _gridHolder = GetComponentInParent<GridHolder>();
        _lockAmountText = GetComponentInChildren<TextMeshProUGUI>();
        _animator = GetComponentInChildren<Animator>();
         _collider = GetComponent<Collider>();
        if (IsLockedOpened != 0)
        {
            _gridHolder.ColliderState(true);
            gameObject.SetActive(false);
        }
        else
        {
            _gridHolder.isLockActive = true;
        }

        mbp = new MaterialPropertyBlock();
    }
    private void OnEnable()
    {
        EventManager.CoreEvents.CheckIfGridsUnlockable += CheckIfGridsUnlockable;
    }
    private void OnDisable()
    {
        EventManager.CoreEvents.CheckIfGridsUnlockable -= CheckIfGridsUnlockable;
    }

    private void Start()
    {
        _lockAmountText.text = _lockUnlockAmount.ToString();
        CheckIfGridsUnlockable();
    }

    public void SetColor(Color32 color)
    {
        mbp.SetColor("_BaseColor", color);
        mesh.SetPropertyBlock(mbp);
    }

    private void CheckIfGridsUnlockable()
    {
        if (CurrencyManager.Instance.GoldAmount>=_lockUnlockAmount)
        {
            SetColor(green);
            _rewardedImage.SetActive(false);
            _coinImage.SetActive(true);
        }
        else
        {
            SetColor(grey);
            _rewardedImage.SetActive(true);
            _coinImage.SetActive(false);
        }
    }

    private void OnMouseDown()
    {
        if (CurrencyManager.Instance.GoldAmount>=_lockUnlockAmount && IsLockedOpened==0)
        {
            IsLockedOpened = 1;
            _gridHolder.ColliderState(true,true);
            _gridHolder.isLockActive=false;
            LockUnlockedAnim();
            CurrencyManager.Instance.TakeGold(_lockUnlockAmount);
          //  EventHandler.Instance.LogEconomyEvent(_lockUnlockAmount, 0, "SlotUnlocked");
        }
        else
        {
            LockRewarded();
        }
    }

    public void LockRewarded()
    {
        IsLockedOpened = 1;
        _gridHolder.ColliderState(true,true);
        _gridHolder.isLockActive=false;
        LockUnlockedAnim();
    }

    // public void CheckIfLockUnlocked()
    // {
    //     if (_isLockRewarded) return;
    //     if (LevelManager.Instance.CollectedHexagonCount >=  _lockUnlockAmount && IsLockedOpened==0)
    //     {
    //         IsLockedOpened = 1;
    //         _gridHolder.ColliderState(true,true);
    //         _gridHolder.isLockActive=false;
    //         LockUnlockedAnim();
    //     }
    // }

    public void LockUnlockedAnim()
    {
        _animator.SetTrigger("LockOpened");
        AudioManager.Instance.Play(AudioManager.AudioEnums.LockUnlocked,.5f);
        DOVirtual.DelayedCall(2f,()=>gameObject.SetActive(false));
    }
}
