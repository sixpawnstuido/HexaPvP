using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AvatarSelect : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private Image flag;

    [SerializeField] private string name;
    
    public void Customize()
    {
        var visualInfo = ResourceSystem.ReturnVisualData();
        var randomValue = Random.Range(0, visualInfo.oppIcons.Count);
        
        var oppIcon = visualInfo.oppIcons[randomValue];
        var oppFlag= visualInfo.oppFlagIcons[randomValue];
        var oppName= visualInfo.oppNames[randomValue];

        icon.sprite = oppIcon;
        flag.sprite = oppFlag;
        name = oppName;
    }
}
