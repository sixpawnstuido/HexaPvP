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

    [SerializeField] private Ease startPosToHexagonEase;
    [SerializeField] private Ease hexagonToGridEase;
    [SerializeField] private Ease gridToHexagonEase;
    [SerializeField] private Ease goBackToStartPosEase;

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
        transform
            .DOMove(hexagonPos,startPosToHexagonDuration)
            .SetEase(startPosToHexagonEase);
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
     //   hexagonHolder.transform.position = new Vector3(hexagonHolder.transform.position.x,hexagonHolder.transform.position.y+1.5f,hexagonHolder.transform.position.z-.86f);
        Vector3 hexagonPos = gridHolder.transform.position +new Vector3(0,1.5f,-.86f);
        transform
            .DOMove(hexagonPos,hexagonToGridDuration)
            .SetEase(hexagonToGridEase);
        yield return new WaitForSeconds(hexagonToGridDuration);
        var levelHolder = LevelManager.Instance.CurrentLevel;
        hexagonHolder.transform.SetParent(levelHolder.transform);
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
        transform
            .DOMove(hexagonPos,gridToHexagonDuration)
            .SetEase(gridToHexagonEase);
        yield return new WaitForSeconds(gridToHexagonDuration);
    }


    public void GoBackToStartPos()
    {
        transform
            .DOMove(_firstPos, goBackToStartPosDuration)
            .SetEase(goBackToStartPosEase);
    }
}
