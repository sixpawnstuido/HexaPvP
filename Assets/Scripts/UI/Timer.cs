using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public static Timer Instance;

    [SerializeField] private TextMeshProUGUI _timerText;
    [SerializeField] private Color32 _timerTextRedColor;
    [SerializeField] private Color32 _timerDefaultColor;

    [ReadOnly] public bool canUpdate = true;
    private bool _isTextRed;

    [ListDrawerSettings(ShowIndexLabels =true)] [SerializeField] private List<float> _timerCountList;
    private float _timerCount;
    private float _levelTimer;

    [SerializeField] private SlicedFilledImage _barFill;
    [SerializeField] private GameObject _timerWarning;

    
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        UpdateTimerCount();
    }

    void Update()
    {
        if (canUpdate)
        {
            if (_timerCount > 0)
            {
                _timerCount -= Time.deltaTime;
                UpdateTimer(_timerCount);
                _barFill.fillAmount = Mathf.InverseLerp(0, _levelTimer, _timerCount);
            }
            else
            {
                canUpdate = false;
                _timerCount = 0;
                DOVirtual.DelayedCall(1, () =>
                {
                    if (LevelManager.IsLevelCompleted) return;
                    LevelManager.Instance.OpenFailedPanel(true);
                });
            }
        }
    }

    private void UpdateTimer(float currentTime)
    {
        currentTime += 1;

        float minutes = Mathf.FloorToInt(currentTime / 60);
        float seconds = Mathf.FloorToInt(currentTime % 60);
        _timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        if (currentTime <= 10)
        {
            if (_isTextRed) return;
            AudioManager.Instance.Play(AudioManager.AudioEnums.TickTock);
            ChangeTextColor(_timerTextRedColor, .5f);
            _isTextRed = true;
            _timerWarning.SetActive(true);
        }
    }

    public void UpdateTimerCount()
    {
        _timerCount = _timerCountList[LevelManager.Instance.LevelCount];
        _levelTimer = _timerCount;
    }

    public void ResetTimer()
    {
        UpdateTimerCount();
        canUpdate = true;
        ChangeTextColor(Color.white, 0f);
        _isTextRed = false;
        _timerWarning.SetActive(false);
    }

    public void AddTime(float time)
    {
        _timerCount = time;
        canUpdate = true;
        ChangeTextColor(Color.white, 0f);
        _isTextRed = false;
        _timerWarning.SetActive(false);
    }

    private void ChangeTextColor(Color targetColor, float duration)
    {
        _timerText.DOColor(targetColor, duration);
    }
}