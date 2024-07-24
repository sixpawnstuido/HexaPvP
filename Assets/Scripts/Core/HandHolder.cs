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
    public float hexagonHolderJumpDuration;
    public float goBackToStartPosDuration;

    public HandState handState;

    [SerializeField] private Transform handVisual;

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
        hexagonSlot.hexagonHolder.transform.SetParent(transform);
    }


    public void HexagonToGridHolder(GridHolder gridHolder=null,HexagonHolder hexagonHolder=null)
    {
        StartCoroutine(HexagonToGridHolderCor(gridHolder,hexagonHolder));
    }

    IEnumerator HexagonToGridHolderCor(GridHolder gridHolder=null,HexagonHolder hexagonHolder=null)
    {
        handVisual
            .DOScale(handSelectScale, .1f)
            .SetEase(Ease.OutBack);
        hexagonHolder.transform.SetParent(transform);
        Vector3 hexagonPos = gridHolder.transform.position + Vector3.up;
        transform.DOMove(hexagonPos,hexagonToGridDuration);
        yield return new WaitForSeconds(hexagonToGridDuration);
        hexagonHolder.transform.SetParent(null);
        hexagonHolder.hexagonCollider.enabled = false;
        Vector3 gridOffset = new Vector3(0, 0.05f, 0);
        hexagonHolder.transform.DOMove(gridHolder.transform.position + gridOffset,hexagonHolderJumpDuration);
        handVisual
            .DOScale(1, .1f)
            .SetEase(Ease.OutBack);
        yield return new WaitForSeconds(hexagonHolderJumpDuration);
        hexagonHolder.HexagonPlacedState(gridHolder,false,true);
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


    public void GoBackToStartPos()
    {
        transform.DOMove(_firstPos, goBackToStartPosDuration);
    }
}
