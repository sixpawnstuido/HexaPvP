using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextEffectsMeta : MonoBehaviour
{
    public static TextEffectsMeta Instance;

    public List<GameObject> holders;

    private int index;


    private void Awake()
    {
        Instance = this;
    }

    public void PlayEffect(Vector3 pos)
    {
        holders[index].SetActive(false);
        holders[index].transform.position = pos;
        holders[index].SetActive(true);

        index++;

        if (index > 3) index = 0;
    }

}
