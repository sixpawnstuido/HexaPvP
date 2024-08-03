using DG.Tweening;
using LeTai.TrueShadow.Demo;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public enum ToggleType
{
    SFX,
    HAPTIC,
    MUSIC
}
public class GameSettingsButton : MonoBehaviour
{

    [SerializeField] private ToggleType _toggleType;

    [SerializeField] private Button _button;

    [Header("SlideValues")]
    [SerializeField]
    private RectTransform toggleCircle;
    [SerializeField]
    private GameObject offSpriteToggle;
    [SerializeField]
    private float toggleCircleTargetXPos;
    [SerializeField]
    private float toggleCircleFirstScale;
    [SerializeField]
    private Color32 toggleCircleOnColor;
    [SerializeField]
    private Color32 toggleCircleOffColor;

    public int IsOn
    {
        get => PlayerPrefs.GetInt(gameObject.name + "Toggle", 1);
        set => PlayerPrefs.SetInt(gameObject.name + "Toggle", value);
    }



    public void Start()
    {
        UpdateSettingsPanel();
        _button.onClick.AddListener(()=>UpdateSettingsPanel());
    }
    private void UpdateSettingsPanel()
    {
        if (IsOn==0)
        {
            ToggleState(true,toggleCircleTargetXPos,toggleCircleOnColor);
            IsOn = 1;

            if(_toggleType==ToggleType.SFX) AudioManager.Instance.SoundOnOffCheck = 1;
            else if(_toggleType == ToggleType.HAPTIC) AudioManager.Instance.VibroOnOffCheck= 1;
            else if (_toggleType == ToggleType.MUSIC)
            {
                AudioManager.Instance.MusicOnOffCheck= 1;
                AudioManager.Instance.StopBGMusic();
            }
        }
        else
        {
            ToggleState(false, -toggleCircleTargetXPos, toggleCircleOffColor);
            IsOn = 0;

            if (_toggleType == ToggleType.SFX) AudioManager.Instance.SoundOnOffCheck = 0;
            else if (_toggleType == ToggleType.HAPTIC) AudioManager.Instance.VibroOnOffCheck = 0;
            else if (_toggleType == ToggleType.MUSIC)
            {
                AudioManager.Instance.MusicOnOffCheck= 0;
                AudioManager.Instance.PlayBGMusic();
            }
        }
    }

    private void ToggleState(bool state, float targetXPos, Color32 toggleCircleColor)
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(toggleCircle.DOAnchorPosX(targetXPos, .1f));
        seq.Join(toggleCircle.GetComponent<Image>().DOColor(toggleCircleColor, .1f));
        seq.Append(toggleCircle.DOScale(toggleCircleFirstScale * 1.15f, .1f));
        seq.Append(toggleCircle.DOScale(toggleCircleFirstScale, .1f));
        offSpriteToggle.SetActive(state);
    }
}
