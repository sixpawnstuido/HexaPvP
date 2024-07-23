using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameLoading : MonoBehaviour
{
    public static InGameLoading Instance;

    [SerializeField] private GameObject holder;

    private void Awake()
    {
        Instance = this;
    }

    public void OpenHolder()
    {
        holder.SetActive(true);
    }

    public void CloseHolder()
    {
        holder.SetActive(false);
    }
}
