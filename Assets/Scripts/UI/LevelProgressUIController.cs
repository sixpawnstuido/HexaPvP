using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class LevelProgressUIController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _levelText;

    private void Start()
    {
        ChangeLevelProgressText(LevelManager.Instance.TotalLevelCount);
    }
    public void ChangeLevelProgressText(int levelCount)
    {
        _levelText.text = _levelText.text = $"Lv. {levelCount}"; ;
    }
}
