using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PvPController : MonoBehaviour
{
    public static PvPController Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject); 
        }
    }


    public void SelectFirstPlayer()
    {

    }
}
