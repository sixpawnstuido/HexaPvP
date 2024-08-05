using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AvatarSelect : MonoBehaviour
{
    public static AvatarSelect Instance;
    
    [SerializeField] private Image icon;
    [SerializeField] private Image flag;

    [SerializeField] private TextMeshProUGUI name;


    private void Awake()
    {
        Instance = this;
    }

    public void Customize()
    {
        var visualInfo = ResourceSystem.ReturnVisualData();
        int levelCount = LevelManager.Instance.LevelCount;
        var oppIcon = visualInfo.oppIcons[levelCount-1];
        var oppFlag= visualInfo.oppFlagIcons[levelCount-1];
        var oppName= visualInfo.oppNames[levelCount-1];

        icon.sprite = oppIcon;
        flag.sprite = oppFlag;
        name.SetText(oppName);
    }
}
