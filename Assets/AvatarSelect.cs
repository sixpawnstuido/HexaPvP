using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

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

    //private void OnEnable()
    //{
    //    Customize();
    //}

    public void Customize()
    {
        var visualInfo = ResourceSystem.ReturnVisualData();
        int randomIndex = Random.Range(0, visualInfo.oppIcons.Count - 1);
        int randomNameIndex = Random.Range(0, visualInfo.oppNames.Count - 1);
        
        var oppIcon = visualInfo.oppIcons[randomIndex];
        //var oppFlag= visualInfo.oppFlagIcons[randomIndex];
        //var oppName= visualInfo.oppNames[randomNameIndex];

        icon.sprite = oppIcon;
        //flag.sprite = oppFlag;
        //name.SetText(oppName);
        

       // AvatarSelectInGame.Instance.Customize(oppIcon,oppFlag,oppName);
    }
}
