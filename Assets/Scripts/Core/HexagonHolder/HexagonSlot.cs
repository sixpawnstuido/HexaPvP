using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexagonSlot : MonoBehaviour
{
    [ReadOnly] public HexagonHolder hexagonHolder;

    public void Init(HexagonHolder hexagonHolder)
    {
        this.hexagonHolder = hexagonHolder;
    }
}
