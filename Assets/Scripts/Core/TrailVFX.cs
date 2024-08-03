using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

public class TrailVFX : MonoBehaviour
{
    
    [SerializeField] private SpriteRenderer hexagonSprite;
    [SerializeField] private TrailRenderer trailRenderer;
    
    [SerializeField] private List<AnimationCurve> trailMotionCurveList;
    

    public void TrailMotion(PlayerType playerType,int hexagonElementAmount,Color hexagonColor,int comboStage=1,int trailCurveIndex=0)
    {
        float time = 0;
        var avatarElement = PvPController.Instance.ReturnAvatarElement(playerType);
        var avatarTarget = avatarElement.HeartImage;
        hexagonSprite.transform.localPosition = Vector3.zero;
        hexagonSprite.enabled = true;
        ChangeColor(hexagonColor);
        int trailCurveIndexClamped=Mathf.Min(trailCurveIndex, trailMotionCurveList.Count - 1);
        var animationCurve = trailMotionCurveList[trailCurveIndexClamped];
        hexagonSprite.transform.DOMove(avatarTarget.transform.position, .55f)
            .OnUpdate(() =>
            {
                time += Time.deltaTime * (1 / .55f);
                float xOffset = animationCurve.Evaluate(time);
                hexagonSprite.transform.position = new Vector3(hexagonSprite.transform.position.x - xOffset, hexagonSprite.transform.position.y, hexagonSprite.transform.position.z);
            })
            .OnComplete(() =>
            {
                avatarElement.TrailArrivedState(hexagonElementAmount,comboStage);
                PvPController.Instance.DecreaseHealth(playerType, hexagonElementAmount,comboStage);
                hexagonSprite.enabled = false;
                DOVirtual.DelayedCall(1, () => gameObject.SetActive(false));
            });
    }



    public void ChangeColor(Color color)
    {
        //var mainModule = trailVFX.main;
    //    mainModule.startColor = color;
        hexagonSprite.color = color;
        var endColor = color;
        endColor.a = 0;
        SetTrailColor(color,endColor);
    }
    
    public void SetTrailColor(Color startColor,Color endColor)
    {
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(startColor, 0.0f), new GradientColorKey(endColor, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(startColor.a, 0.0f), new GradientAlphaKey(endColor.a, 1.0f) }
        );
        trailRenderer.colorGradient = gradient;
    }
}
