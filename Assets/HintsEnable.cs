using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HintsEnable : MonoBehaviour
{
    public static HintsEnable Instance;

    [SerializeField] private GameObject hint1;
    [SerializeField] private GameObject hint2;


    private void Awake()
    {
        Instance = this;    
    }

    private void Start()
    {
        if (LevelManager.Instance.LevelCount > 1)
        {
            OpenHints();
        }
        else CloseHints();
    }

    public void CloseHints()
    {
        hint1.SetActive(false);
        hint2.SetActive(false);
    }

    public void OpenHints()
    {
        hint1.SetActive(true);
        hint2.SetActive(true);
    }
}
