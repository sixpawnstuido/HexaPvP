using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RestartButton : MonoBehaviour
{
    public static RestartButton Instance;
    private Button _restartButton;

    [SerializeField] private GameObject _holder;

    private void Awake()
    {
        Instance = this;
        
        _restartButton = GetComponentInChildren<Button>();
    }

    private void Start()
    {
        _restartButton.onClick.AddListener(OnRestartButtonClick);
        if(LevelManager.Instance.LevelCount==1) HolderSetactiveState(false);
    }

    private void OnRestartButtonClick()
    {
        UIManager UIManager=UIManager.Instance;
        if (UIManager.nextLevelPanel.gameObject.activeInHierarchy 
            || UIManager.failedPanel.gameObject.activeInHierarchy
            || TutorialManager.TutorialCompleted==0) return;
        
        LevelManager.Instance.RestartLevel();
    }

    public void HolderSetactiveState(bool state) => _holder.SetActive(state);
}
