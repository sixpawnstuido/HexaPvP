using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using DG.Tweening;
using UnityEngine;


public enum HandState
{
    StartPos,
    Moving,
}
public class HandHolder : MonoBehaviour
{
    private Vector3 _firstPos;

    [Header("Hand Animation Infos")]
    [SerializeField] private Vector3 handSelectScale;

    public float startPosToHexagonDuration;
    public float hexagonToGridDuration;
    public float gridToHexagonDuration;

    public HandState handState;

    private void Awake()
    {
        _firstPos = transform.position;
    }

    public void StartPosToHexagonHolder(HexagonSlot hexagonSlot = null)
    {
        StartCoroutine(StartPosToHexagonHolderCor(hexagonSlot));
    }
    IEnumerator StartPosToHexagonHolderCor(HexagonSlot hexagonSlot = null)
    {
        handState = HandState.Moving;
        Vector3 hexagonPos = hexagonSlot.transform.position + Vector3.up;
        transform.DOMove(hexagonPos,startPosToHexagonDuration);
        yield return new WaitForSeconds(startPosToHexagonDuration);
        transform
            .DOScale(handSelectScale, .1f)
            .SetEase(Ease.OutBack);
    }


    public void HexagonToGridHolder(GridHolder gridHolder=null)
    {
        StartCoroutine(HexagonToGridHolderCor(gridHolder));
    }

    IEnumerator HexagonToGridHolderCor(GridHolder gridHolder=null)
    {
        Vector3 hexagonPos = gridHolder.transform.position + Vector3.up;
        transform.DOMove(hexagonPos,hexagonToGridDuration);
        yield return new WaitForSeconds(hexagonToGridDuration);
    }
    
    
    
    public void GridToHexagonHolder(HexagonHolder hexagonHolder=null)
    {
        StartCoroutine(GridToHexagonHolderCor(hexagonHolder));
    }

    IEnumerator GridToHexagonHolderCor(HexagonHolder hexagonHolder=null)
    {
        Vector3 hexagonPos = hexagonHolder.transform.position + Vector3.up;
        transform.DOMove(hexagonPos,gridToHexagonDuration);
        yield return new WaitForSeconds(gridToHexagonDuration);
    }
}
