using System;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using DG.Tweening;
using GameAnalyticsSDK.Setup;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JuiceTargetUIElement : SerializedMonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _juiceCountText;
    [SerializeField] private TextMeshProUGUI _juiceNameText;

    [SerializeField] private Image _juiceImage;
    [SerializeField] private Image _juiceColor;

    [ReadOnly] public HexagonTypes hexagonType;

    private JuiceTargetUIController _juiceTargetUIController;

    [SerializeField] private GameObject _checkmark;
    [SerializeField] private ParticleSystem _foundedVFX;
    [SerializeField] private GameObject _nameHolder;

    public Dictionary<HexagonTypes, GlassInfo> GlassInfos;

    public Transform glasRef;

    [Serializable]
    public class GlassInfo
    {
        public Color32 color;
        public Sprite fruitSprite;
    }

    public bool IsJuiceTargetActivated
    {
        get => Convert.ToBoolean(
            PlayerPrefs.GetInt("IsJuiceTargetActivated" + LevelManager.Instance.LevelCount + hexagonType, 0));
        set => PlayerPrefs.SetInt("IsJuiceTargetActivated" + LevelManager.Instance.LevelCount + hexagonType,
            Convert.ToInt32(value));
    }

    public int JuiceBoxCount
    {
        get => PlayerPrefs.GetInt("JuiceBoxCount" + LevelManager.Instance.LevelCount + hexagonType, 1);
        set => PlayerPrefs.SetInt("JuiceBoxCount" + LevelManager.Instance.LevelCount + hexagonType, value);
    }

    private void Awake()
    {
        _juiceTargetUIController = GetComponentInParent<JuiceTargetUIController>();
        JuiceBoxCount = JuiceBoxCount;
    }

    private void Start()
    {
        if (JuiceBoxCount <= 0) // if founded
        {
            // LevelManager.Instance.ReturnHexagonSpawner().RemoveFromHexagonTypesList(hexagonType);
            _checkmark.transform.DOScale(Vector3.one, 0f);
            _juiceCountText.gameObject.SetActive(false);
        }
        
        _juiceNameText.SetText(hexagonType.ToString());
    }

    public void SetJuiceCountText() => _juiceCountText.SetText($"x{JuiceBoxCount}");
    public void SetJuiceImage(Sprite sprite,Color32 color)
    {
        _juiceImage.sprite = sprite;
        _juiceColor.color = color;


    }
    public void SetHexagonType(HexagonTypes hexagonType) => this.hexagonType = hexagonType;

    public void DecreaseJuiceCount()
    {
        JuiceBoxCount--;
        JuiceBoxCount = (int)Mathf.Clamp(JuiceBoxCount, 0, float.MaxValue);
        SetJuiceCountText();
        if (JuiceBoxCount <= 0)
        {
            JuiceTargetCompletedState();
        }
        else
        {
            DecreaseAnim();
        }
    }

    public void JuiceTargetCompletedState()
    {
        //  LevelManager.Instance.ReturnHexagonSpawner().RemoveFromHexagonTypesList(hexagonType);
        FoundedAnim();
        _juiceTargetUIController.CheckIfAllTargetsCompleted();
    }

    private void FoundedAnim()
    {
        _juiceColor.transform.parent.DOPunchScale(.2f * Vector3.one, .3f, 3);
        _checkmark.transform.DOScale(Vector3.one, .2f).SetEase(Ease.OutBack);
        _juiceCountText.gameObject.SetActive(false);
        AudioManager.Instance.Play(AudioManager.AudioEnums.TargetCompleted);
        _foundedVFX.Stop();
        _foundedVFX.Play();
    }

    private void DecreaseAnim()
    {
        _juiceColor.transform.parent.DOPunchScale(.2f * Vector3.one, .3f, 3);
        AudioManager.Instance.Play(AudioManager.AudioEnums.Succes1);
        _foundedVFX.Stop();
        _foundedVFX.Play();
    }

    public void NameHolderScale(float scale,float duration,Ease ease)
    {
        // _nameHolder.transform
        //     .DOScale(scale, duration)
        //     .SetEase(ease);
    }
}