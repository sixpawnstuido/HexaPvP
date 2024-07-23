using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FruitAnimTest : MonoBehaviour
{
    [Button]
    public void AnimTest()
    {
        transform.DOJump(transform.position, 1, 1, .1f).SetLoops(-1, LoopType.Incremental);

        transform.DORotate(360 * Vector3.up, .5f, RotateMode.FastBeyond360).SetLoops(-1, LoopType.Incremental).SetEase(Ease.Linear);
        
    }
}
