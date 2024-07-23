using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    public GameObject[] objects;

    private bool enabled2;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) enabled2 = !enabled2;

        foreach(GameObject g in objects)
        {
            g.SetActive(enabled2);
        }
    }
}
