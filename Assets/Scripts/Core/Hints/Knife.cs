using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knife : MonoBehaviour
{
    private HexagonHolder _hexagonHolder;
    public void Init(HexagonHolder hexagonHolder) => _hexagonHolder = hexagonHolder;

    public void KnifeAnimEnd()
    {
        EventManager.CoreEvents.GridHolderColliderState(false,false);
    }
}
