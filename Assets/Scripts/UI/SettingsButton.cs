using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsButton : MonoBehaviour
{
    [SerializeField] private GameObject _holder;

    [SerializeField] private Button _settingsButton;
    [SerializeField] private Button _closeButton;

    private void Start()
    {
        _settingsButton.onClick.AddListener(() =>_holder.SetActive(true));
        _closeButton.onClick.AddListener(() =>_holder.SetActive(false));
    }
}
