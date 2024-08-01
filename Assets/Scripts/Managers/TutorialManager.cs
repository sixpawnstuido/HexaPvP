using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;

    [ShowInInspector, ReadOnly]
    public static int TutorialCompleted
    {
        get => PlayerPrefs.GetInt("TutorialCompleted", 0);
        set => PlayerPrefs.SetInt("TutorialCompleted", value);
    }

    private void Awake()
    {
        Instance = this;
        if (TutorialCompleted == 0)
        {
            PlayerPrefs.DeleteAll();
        }
    }
    
    


   
}
