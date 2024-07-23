using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NextLevelPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _levelCompletedText;
    [SerializeField] private Button _nextLevelButton;

    private void Awake()
    {
        _nextLevelButton = GetComponentInChildren<Button>(true);
    }
    private void OnEnable()
    {
       // ChangeLevelCompletedText(LevelManager.Instance.LevelCount);
    }
    private void Start()
    {
        _nextLevelButton.onClick.AddListener(()=>
        {
            LevelManager.IsLevelCompleted = false;
        });
    }

    public void ChangeLevelCompletedText(int levelIndex)
    {
        _levelCompletedText.SetText($"Level {levelIndex-1} Completed");
    }
}
