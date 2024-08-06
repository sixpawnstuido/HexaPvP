using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class ExtraMove : MonoBehaviour
{
    [ReadOnly] public ExtraMovePool extraMovePool;
    
    public void Init()
    {
        transform.localScale=Vector3.zero;
        transform.position = extraMovePool.transform.position;
        
        float yPos = extraMovePool.extraMoveTargetY;
        
        Sequence seq = DOTween.Sequence();
        seq.Append(transform
            .DOLocalMoveY(yPos, .2f)
            .SetEase(Ease.OutBack));

        seq.Join(transform
            .DOScale(1f, .2f)
            .SetEase(Ease.OutBack));

        seq.AppendInterval(1f);

        seq.Append(transform
            .DOScale(0f, .3f)
            .SetEase(Ease.InBack));

        seq.AppendCallback(() => gameObject.SetActive(false));
    }
}