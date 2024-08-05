using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AvatarSelectInGame : MonoBehaviour
{
    public static AvatarSelectInGame Instance;
    
    [SerializeField] private Image icon;
    [SerializeField] private Image flag;

    [SerializeField] private TextMeshProUGUI name;

    private void Awake()
    {
        Instance = this;
    }

    public void Customize(Sprite icon,Sprite flag,string name)
    {
        this.icon.sprite = icon;
        this.flag.sprite = flag;
        this.name.SetText(name);
    }
}
