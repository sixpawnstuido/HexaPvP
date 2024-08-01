using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class TrailVFX : MonoBehaviour
{

    [SerializeField] private ParticleSystem trailVFX;
    
    [SerializeField] private List<AnimationCurve> trailMotionCurveList;
    
    [SerializeField] private Color32 blueColor;
    [SerializeField] private Color32 redColor;
    

    public void TrailMotion(PlayerType playerType,int hexagonElementAmount,Color hexagonColor,int comboStage=1)
    {
        float time = 0;
        var avatarElement = PvPController.Instance.ReturnAvatarElement(playerType);
        var avatarTarget = avatarElement.HeartImage;
        trailVFX.transform.localPosition = Vector3.zero;
        ChangeColor(hexagonColor);
        var animationCurve = trailMotionCurveList[Random.Range(0, trailMotionCurveList.Count)];
        trailVFX.transform.DOMove(avatarTarget.transform.position, .45f)
            .OnUpdate(() =>
            {
                time += Time.deltaTime * (1 / .45f);
                float xOffset = animationCurve.Evaluate(time);
                trailVFX.transform.position = new Vector3(trailVFX.transform.position.x - xOffset, trailVFX.transform.position.y, trailVFX.transform.position.z);
            })
            .OnComplete(() =>
            {
                avatarElement.TrailArrivedState(hexagonElementAmount,comboStage);
                PvPController.Instance.DecreaseHealth(playerType, hexagonElementAmount,comboStage);
                DOVirtual.DelayedCall(1, () => gameObject.SetActive(false));
            });
    }



    public void ChangeColor(Color color)
    {
        var mainModule = trailVFX.main;
        mainModule.startColor = color;
    }
}
