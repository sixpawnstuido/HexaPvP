using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public abstract class HintBase : MonoBehaviour
{
    [ShowInInspector]
    public int HintCount
    {
        get => PlayerPrefs.GetInt("HintCount"+hintType.ToString(), 1);
        set => PlayerPrefs.SetInt("HintCount" + hintType.ToString(), value);
    }
    [ShowInInspector]
    public int HintActivated
    {
        get => PlayerPrefs.GetInt("HintActivated" + hintType.ToString(), 0);
        set => PlayerPrefs.SetInt("HintActivated" + hintType.ToString(), value);
    }

    public HintType hintType;

    public Button hintButton;

    public GameObject _lockImage;
    [SerializeField] private GameObject _hintCountHolder;
    [SerializeField] private GameObject unlockedHolder;
    public GameObject _plusHolder;
    public GameObject buyPanel;
    public GameObject tutorial;

    [SerializeField] private TextMeshProUGUI _hintCountText;

    public int hintLevelIndex;

    [ReadOnly] public bool isHintActive;

    public CanvasGroup focusCanvasGroup;
    public CanvasGroup hintParentCanvasGroup;
    
    public int hintCost;

    public GameObject secondCam;

    public virtual void UnlockHint()
    {
        HintActivated = 1;
        UpdateHintState();
    }

    protected virtual void Start()
    {
        UpdateHintState();
    }

    public void UpdateHintState()
    {
        if (HintActivated == 0)
        {
            _lockImage.SetActive(true);
            unlockedHolder.SetActive(false);
            _plusHolder.SetActive(false);
            _hintCountHolder.SetActive(false);
            return;
        }

        _lockImage.SetActive(false);
        unlockedHolder.SetActive(true);

 

        if (HintCount > 0)
        {
            _hintCountHolder.SetActive(true);
            _plusHolder.SetActive(false);

        }
        else
        {
            _hintCountHolder.SetActive(false);
            _plusHolder.SetActive(true);
        }

        _hintCountText.text = HintCount.ToString();
    }

    public void HintScalePunch()
    {
        transform.DOPunchScale(.2f * Vector3.one, .3f, 3);
    }

    public void DecreaseMoveCount()
    {
        HintCount--;

        UpdateHintState();
    }

    public void EarnHint()
    {
        HintCount++;
        CurrencyManager.Instance.TakeGold(hintCost);
        UpdateHintState();
    }

    public void SetGameLayerRecursive(GameObject _go, int _layer)
    {
        _go.layer = _layer;
        foreach (Transform child in _go.transform)
        {
            child.gameObject.layer = _layer;

            Transform _HasChildren = child.GetComponentInChildren<Transform>();
            if (_HasChildren != null)
                SetGameLayerRecursive(child.gameObject, _layer);

        }
    }


}
public enum HintType
{
    ClearSlot = 0,
    ChangeSlot = 1,
}
