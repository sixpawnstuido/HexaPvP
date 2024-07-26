using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class TrailVFX : MonoBehaviour
{

    [SerializeField] private ParticleSystem trailVFX;
    
    [SerializeField] private AnimationCurve trailMotionCurve;
    [SerializeField] private List<AnimationCurve> trailMotionCurveList;



    public void Init()
    {
        
    }

    [Button]
    public void TrailMotion()
    {
        float time = 0;
        var avatarElement = PvPController.Instance.ReturnTarget();
        var avatarTarget = avatarElement.HeartImage;
        trailVFX.transform.DOMove(avatarTarget.transform.position, .45f)
            .OnUpdate(() =>
            {
                time += Time.deltaTime * (1 / .45f);
                float xOffset = trailMotionCurve.Evaluate(time);
                trailVFX.transform.position = new Vector3(trailVFX.transform.position.x - xOffset, trailVFX.transform.position.y, trailVFX.transform.position.z);
            })
            .OnComplete(() =>
            {
                if (!DOTween.IsTweening(avatarTarget.transform.GetHashCode()))
                {
                  avatarTarget.transform
                      .DOPunchScale(new Vector3(-.15f, 0.15f, 0), .3f, 10) 
                      .SetId(avatarTarget.transform.GetHashCode());
                }
                gameObject.SetActive(false);
                //DOVirtual.DelayedCall(0.5f, () => gameObject.SetActive(false));
            });
    }
}
