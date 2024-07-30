using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using Sirenix.Utilities.Editor;
using UnityEngine;

public class OrderOfPlayPanel : SerializedMonoBehaviour
{
    public static OrderOfPlayPanel Instance;
    
    [SerializeField] private Dictionary<PlayerType, GameObject> panels;


    private void Awake()
    {
        Instance = this;
    }

    public void PanelState(PlayerType playerType)
    {
       var panel=panels[playerType];
       panel.SetActive(true);
       DOVirtual.DelayedCall(1f, ()=>panel.SetActive(false));
    }
}
